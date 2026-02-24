library(shiny)
library(shinyjs)
library(shinyBS)
library(dplyr)
require(ggplot2)
require(gridExtra)
require(reshape)
source("rest.r")
source("plots.r")

gg.gauge <- function(pos,breaks=c(0,150,300,450,600)) {
  pos <- round(pos)
  get.poly <- function(a,b,r1=0.5,r2=1.0) {
    th.start <- pi*(1-a/600)
    th.end   <- pi*(1-b/600)
    th       <- seq(th.start,th.end,length=600)
    x        <- c(r1*cos(th),rev(r2*cos(th)))
    y        <- c(r1*sin(th),rev(r2*sin(th)))
    return(data.frame(x,y))
  }
  ggplot()+ 
    geom_polygon(data=get.poly(breaks[1],breaks[2]),aes(x,y),fill="red")+
    geom_polygon(data=get.poly(breaks[2],breaks[3]),aes(x,y),fill="darkorange")+
    geom_polygon(data=get.poly(breaks[3],breaks[4]),aes(x,y),fill="gold")+
    geom_polygon(data=get.poly(breaks[4],breaks[5]),aes(x,y),fill="forestgreen")+
    geom_polygon(data=get.poly(pos-2,pos+2,0.3),aes(x,y))+
    geom_text(data=as.data.frame(breaks), size=6, fontface="bold", vjust=0,
              aes(x=1.1*cos(pi*(1-breaks/600)),y=1.1*sin(pi*(1-breaks/600)),label=breaks))+
    annotate("text",x=0,y=0,label=pos,vjust=0,size=15,fontface="bold")+
    coord_fixed()+
    theme_bw()+
    theme(axis.text=element_blank(),
          axis.title=element_blank(),
          axis.ticks=element_blank(),
          panel.grid=element_blank(),
          panel.border=element_blank()) 
}

percentile_plot <- function(percentile) 
{
  ggplot() +
  geom_col(aes("", 100), fill="#9EABB1") + 
  geom_col(aes("", percentile), fill="#3C5864") + 
  coord_flip() + 
  theme_void()
}

# The session information of the uploaded data point.
read_session <- function(directory){
  path = paste(directory, "upload", "data", "session.csv", sep="\\")
  print(paste0("PATH: ", path))
  
  if (file.exists(path)) {
    print("The file exists!")
  } else {
    print("The file does not exist.")
  }
  
  x <- read.csv(path)
  return (x)
}

#  UI, that what will be displayed in the web page
ui <- fluidPage(
  tags$head(
    tags$link(rel="stylesheet", type="text/css", href="user_css.css")
  ),
  column(width=2,
    fluidRow(img(src="inputlog.png", width="200px")),
    fluidRow(selectInput("languageSelect", "Language:", c("English"="EN", "Nederlands"="NL"))),
    fluidRow(fileInput("fileUpload", "Upload file:", accept=".idfx"))#,
  ),
  column(width=8,
    wellPanel(
      fluidRow(
        column(offset=1, width=5,
          htmlOutput("personal")
        ),
        column(offset=1, width=5,
          htmlOutput("task")
        )
      )
    ),
    fluidRow(
      column(offset=1, width=5,
        fluidRow(
          column(width=12, htmlOutput("formulaOut"), align="center")
        ),
        fluidRow(
          column(width=12, plotOutput("speedometerPlot", height=250))
        ),
        fluidRow(
          column(width=12, h3("Score on 0 to 600 scale"), align="center")
        )
      ),
      column(offset=1, width=5,
        fluidRow(
          column(width=12, h1(textOutput("animalNameOut"), align="center"))
        ),
        fluidRow(
          div(imageOutput("animalImageOut", height = 250), style="text-align: center;")
        ),
        fluidRow(
          h3(textOutput("animalDescriptionOut"))
        )
      )
    ),
    htmlOutput("comparisonDescriptionOut")
  )
)

#  Server, contains the logic that will be ran when interacting with the ui.
#  We load the session data that is sampled and keep it in memory as it is generally small. Filtering on this data is then done server-side without re-accessing the database (unless the amount of samples is updated).
server <- function(input, output) {
  
  # Create a temporary location for all the data and cleanup after end of session.
  temp_directory = file.path(tempfile())
  dir.create(temp_directory)
  dir.create(paste(temp_directory,"upload",sep="/"))
  
  cleanup <- function(){
    unlink(temp_directory, recursive = TRUE, force = TRUE)
  }
  
  onSessionEnded(cleanup)
  
  values <- reactiveValues(uploaded_session=NULL)
  
  score <- reactive({
    if (!is.null(values$uploaded_session))
    {
      return (round(min(values$uploaded_session$correctness * values$uploaded_session$cpm/100 ,600), digits = 1))
    }
    return(1)
  })
  
  age_range <- reactive({
    if (!is.null(values$uploaded_session))
    {
       lower <- 5*floor(values$uploaded_session$age/5)
       upper <- lower + 4
       return (c(lower, upper))
    }
    return (NULL)
  })
  
  percentile_pop <- reactive({
    if (!is.null(values$uploaded_session))
    {
      data <- random_samples("session", db_count())
      return (round(100* nrow(data[data$score < score(), ]) / nrow(data), 1))
    }
    return (0)
  }) 
    
  percentile_age <- reactive({
    if (!is.null(values$uploaded_session))
    {
      data <- random_samples("session", db_count())
      data_age <- data[data$age >= age_range()[1] & data$age <= age_range()[2], ] 
      return (round(100* nrow(data_age[data_age$score < score(), ]) / nrow(data_age), 1))
    }
    return (0)    
  })

  english <- reactive({
    input$languageSelect == "EN"
  })
  
  output$comparisonPlotPop <- renderPlot({
    return (percentile_plot(percentile_pop()))
  }, bg="transparent")
  
  output$comparisonPlotAge <- renderPlot({
    return (percentile_plot(percentile_age()))
  }, bg="transparent")
  
  output$personal <- renderUI({
    personal <- if (english()) "Personal Information" else "Persoonlijke Informatie"
    name <- if (english()) "Name" else "Naam"
    age <- if (english()) "Age" else "Leeftijd"
    
    if (!is.null(values$uploaded_session))
    {
      session <- values$uploaded_session
      name <- paste(name, ":", session$uuid)
      age <- paste(age, ":", session$age)
    }
    
    return (
      tagList(strong(h3(personal)),
              h5(name),
              h5(age)
             )
      )
  })
  
  observeEvent(input$languageSelect, {
    updateSelectInput(inputId="languageSelect", label = if(english()) "Language" else "Taal", choices = c("English"="EN", "Nederlands"="NL"), selected=input$languageSelect)
    updateRadioButtons(inputId="comparisonGroup", label = if(english()) "Compare to" else "Vergelijk met", choices = if(english()) c("Age Group"=0, "Total Population"=1) else c("Leeftijdsgroep"=0, "Volledige bevolking"=1), selected=input$comparisonGroup, inline=TRUE)
  })
  
  output$task <- renderUI({
    task <- if (english()) "Task Information" else "Taak Informatie"
    date <- if (english()) "Date" else "Datum"
    language <- if (english()) "Language" else "Taal"
    keyboard <- if (english()) "Keyboard" else "Toetsenbord"
    
    if (!is.null(values$uploaded_session))
    {
      session <- values$uploaded_session
      date <- paste(date, ":", strsplit(session$creation_time, ' ')[[1]][1])
      language <- paste(language, ":", session$language)
      keyboard <- paste(keyboard, ":", session$keyboard)
    }
    
    return (
        tagList(strong(h3(task)),
                h5(date),
                h5(language),
                h5(keyboard)
        )
    )
  })
  
  animal <- reactive({
    if (score() < 31)
      return ("penguin")
    else if (score() < 121)
      return ("koala")
    else if (score() < 211)
      return ("rabbit")
    else if (score() < 301)
      return ("monkey")
    else if (score() < 391)
      return ("fox")
    else if (score() < 481)
      return ("kangaroo")
    else if (score() < 571)
      return ("horse")
    
    return ("falcon")
    
  })
  
  animal_nl <- reactive({
    if (animal() == "penguin")
      return ("pinguin")
    if (animal() == "rabbit")
      return ("konijn")
    if (animal() == "monkey")
      return ("aap")
    if (animal() == "fox")
      return ("vos")
    if (animal() == "kangaroo")
      return ("kangoeroe")
    if (animal() == "horse")
      return ("paard")
    if (animal() == "falcon")
      return ("valk")
    return (animal())
  })
  
  output$speedometerPlot <- renderPlot({
    gg.gauge(score())
  }, bg="transparent")
  
  output$animalImageOut <- renderImage(deleteFile=FALSE, {
    return (list(src=paste0("animals/", animal(), ".jpeg")))
  })
  
  output$animalDescriptionOut <- renderText({
    f <- if (english()) animal() else paste0(animal(), "_nl")
    file_name <- paste0("animals/", f, ".txt")
    return (paste0(readLines(file_name, warn=FALSE), collapse="\n"))
  })
  
  output$animalNameOut <- renderText({
    return (if (english()) animal() else animal_nl())
  })
  
  output$formulaOut <- renderUI({
    if (!is.null(values$uploaded_session))
    {
      return (tagList(
                h1(paste(values$uploaded_session$cpm, "X", values$uploaded_session$correctness, "=", score())), 
                h5(HTML(paste(HTML("&nbsp;"), "cpm", HTML(rep("&emsp;", 4)), "accuracy %", HTML(rep("&emsp;", 3)), "cpm(netto)", sep=" ")))
             ))
    }
  })
  

  output$comparisonDescriptionOut <- renderUI({
    if (!is.null(values$uploaded_session))
    {
      text <- if(english()) "Better than" else "Beter dan"
      text2 <- text
    
      text <- paste(text, percentile_pop(), "%")
      text2 <- paste(text2, percentile_age(), "%")
      
      text <- paste(text, if(english()) "of the population" else "van de bevolking")
      text2 <- paste(text2, if(english()) "of your age range" else "van je leeftijdsgroep")
      text2 <- paste(text2, "(",age_range()[1], " - ", age_range()[2], ")")
      
      wellPanel(
        fluidRow(
          column(offset=1, width=6,
            h3(text)
          ),
          column(width=4,
            plotOutput("comparisonPlotPop", height="50px")
          )
        ),
        fluidRow(
          column(offset=1, width=6,
            h3(text2)
          ),
          column(width=4,
            plotOutput("comparisonPlotAge", height="50px")
          )
        )
      )
    }
  })

  observeEvent(input$fileUpload, {
    unlink(paste(temp_directory, "/upload/*", sep=""), force = TRUE, recursive = TRUE)
    withProgress(message = "Executing analysis on the uploaded file.", {
      upload_file_to_server(input$fileUpload$datapath, temp_directory)})  # Ugly code, but we need to send for analysis as soon as the file is uploaded, as this is more intuitive.
  
    values$uploaded_session <- read_session(temp_directory)
	print("TEST")
	print(values$uploaded_session)
	print("TEST")
  })
 }

options(shiny.fullstacktrace = TRUE)
app <- shinyApp(ui, server)
