library(shiny)
library(shinyjs)
library(shinyBS)
library(dplyr)
source("rest.r")
source("plots.r")
source("download.r")

filter_by_conditions <- function(df, colname, conditions)
{
  filtered <- df[0,]  # clear all rows

  for (condition in conditions)
  {
    filtered <- rbind(filtered, df[df[[colname]] == condition, ])  # Filter using a single condition and append to the filtered dataframe.
  }

  return (filtered)
}

# In case of missing values allowed, we first want to extract the non-missing values, to apply filtering to them and then add the missing values back in.
#  Otherwise, we will have some NA values in the rows where filtering was applied on a non-missing item.
filter_by_range <- function(df, colname, conditions, allow_missing)
{
  
  non_missing <- df[complete.cases(df[, colname]), ]  # remove rows from df with NA (missing) values in the specified column.
  missing <- dplyr::anti_join(df, non_missing, by=colnames(df)) # exclude all rows of non_missing from df.
  filtered <- non_missing[ non_missing[[colname]] >= conditions[1] & non_missing[[colname]] <= conditions[2], ]
  if (allow_missing)
  {
    filtered <- rbind(filtered, missing)
  }
  return (filtered)
}

copy_tasks <- c("tapping", "sentence", "words1", "words2", "words3", "words4", "consonants")
hand_combinations <- c("RR", "RL", "LR", "LL")
trials <- c("1" = "trial1", "2" = "trial2", "3" = "trial3", "4" = "trial4", "5" = "trial5", "6" = "trial6", "7" = "trial7", "overall")
frequencies <- c("high", "low")
variables <- c("targetted", "non-targetted"="non_targetted", "cpm", "median", "stdev", "logmean", "mean_iki")

#  UI, that what will be displayed in the web page
ui <- fluidPage(
    shinyjs::useShinyjs(),
    tags$head(
      tags$script(HTML("
        // Enable navigation prompt
        window.onbeforeunload = function() {
            return 'Your changes will be lost!';
        };
        ")
      ),
      tags$link(rel="stylesheet", type="text/css", href="expert_css.css")
    ),
    title = "InputLog Analysis",
    fluidRow(
      column(
        bsCollapse(id= "leftCollapse", open="introCollapsible",
          bsCollapsePanel(title=img(src="inputlog.png", width="128px", height="60px"), value="introCollapsible",
            includeHTML("./introduction.html")
        )
      ),
      width = 2
    ),
    column(
      bsCollapse(id="mainCollapse", open = "Reference Corpus", multiple = TRUE,
        bsCollapsePanel(title="Reference Corpus",
          helpText("Filter the corpus used for analysis based on different criteria"),
          fluidRow(
            column(width=8,
              fluidRow(
                column(width=6, 
                  fluidRow(
                    column(width=12, sliderInput("SampleNr", "Number of Corpus Samples", min = 0, max = 100, value = 100))
                  ),
                  fluidRow(
                    column(width=12, checkboxInput("includeMissingCheck", "Include samples with missing session data", value = TRUE))
                  ),
                  fluidRow(
                    column(width=12, htmlOutput("SampleNr"))
                  ),
                  fluidRow(
                    column(width=12, actionButton("resampleBtn", "Resample"))
                  )
                ),
                column(width=2, checkboxGroupInput("languageCheck", "Languages", choices = c())),
                column(width=2, checkboxGroupInput("genderCheck", "Gender", choices = c())),
                column(width=4, radioButtons("repetitionCheck", "Repetitions", choices = c("Allowed" = -1, "Disallowed" = 0, "Only Repetitions" = 1), selected=c(-1)))
              )
            ),
            column(width=4,
              fluidRow(
                column(width=12, sliderInput("ageSlider", "Age", min=0, max=100, value = c(0, 100))),
              ),
              fluidRow(
                column(width=12, sliderInput("correctnessSlider", "Correctness", min=0, max=100, value = c(0, 100)))
              ),
              fluidRow(
                column(width=12, sliderInput("handednessSlider", "Handedness", min=-100, max=100, value = c(-100, 100)))
              )              
            )
          )
        ),
        bsCollapsePanel(title="Upload Your Own IDFX(s)",
          helpText("The following options can be used to select the data sources for analysis and comparison."),
          radioButtons("uploadType", "", choices = c("Single File" = 1, "Multiple Files" = 2), selected = 1, inline = TRUE, width="800px"),
          fluidRow(
            column(width=2, fileInput("Upload", "Upload Your Own IDFX", accept = ".idfx")),
            column(width=2, offset=2, fileInput("UploadMultiple", "Upload Multiple IDFXs", accept = ".idfx", multiple = TRUE)),
            column(width=2, radioButtons("aggregateType", "Aggregate Method", choices = c("None" = 1, "Mean" = 2, "Median" = 3), selected = 1))
          )
        )
      ),
      tabsetPanel(type="tabs",
        tabPanel("Analysis",
          fluidRow(
            column(width=2, selectInput("typeSelect", "Perspective", choices = c("Component(s)"="component", "Trial(s)"="trial", "Frequency", "Hand combination(s)"="hands", "Adjacency", "Repetition"), selected="component")),
            conditionalPanel("input.typeSelect == 'trial'",column(width=2, selectInput("trialTaskSelect", "Component", choices = c("words1", "words2", "words3", "words4")))),
            column(width=2, selectInput("experimentSelect", "Component(s):", choices = copy_tasks, multiple = TRUE, selected = c("words1", "words2", "words3", "tapping"))),
            column(width=2, selectizeInput("experimentVariableSelect", "Variable:", choices = variables)),
            column(width=2, selectInput("experimentPlotSelect", "PlotType", choices = c("Boxplot", "Histogram", "Scatterplot"))),
            conditionalPanel("input.experimentPlotSelect == 'Histogram'", column(width=2, sliderInput("experimentBinSlider", "Number of Bins", min = 2, max = 20, value = 10))),
            conditionalPanel("input.experimentPlotSelect == 'Scatterplot'", column(width=2, selectizeInput("xValueSelect", "X-Axis Variable", choices = c()))),
            conditionalPanel("input.experimentPlotSelect == 'Boxplot'", column(width=2, selectizeInput("allowOutliersSelect", "Outliers", choices = c("Allowed"=TRUE, "Disallowed"=FALSE))))), 
          fluidRow(
            column(width=10, plotOutput("experimentComparisonPlot")),
          ),
          fluidRow(
            column(width=1, downloadButton("downloadExperimentPlot", "Download this plot"))
          )
        ),
        tabPanel("Corpus Statistics",
          fluidRow(
            column(width=3, selectInput("statsPlotType", "PlotType", choices = c("Histogram", "Pie Chart"))),
            column(width=3, selectizeInput("corpusVarName", "Variable", choices = c())),
            conditionalPanel(condition = "output.isNumericCorpusVariable && input.statsPlotType == 'Histogram'", column(width=3, sliderInput("corpusStatsBinSlider", "Number of Bins", min = 2, max = 10, value = 5)))
          ),
          fluidRow(
            column(width=10, plotOutput("corpusStats"))
          ),
          fluidRow(
            column(width=1, downloadButton("downloadCorpusStats", "Download this plot"))
          )
        )
      ),
      width = 8
    ),
    column(
      bsCollapse(id="rightCollapse", open = "Download Data",
        bsCollapsePanel("Download Data",
            fluidRow(column(width=12, radioButtons("userDownloadOptions", "", choices = c("Only corpus data"=0, "Corpus + user-provided data"=1, "Only user-provided data"=2), selected=0))),
            fluidRow(column(width=12, actionButton("startDownloadWizard", "Start Download Wizard")))
        )
      ),
      width=2
    )
  )
)


#  Server, contains the logic that will be ran when interacting with the ui.
#  We load the session data that is sampled and keep it in memory as it is generally small. Filtering on this data is then done server-side without re-accessing the database (unless the amount of samples is updated).
server <- function(input, output) {

  # Initializing code for each user

  # Create temporary directories for each user and delete them at the end of their session.
  temp_directory = file.path(tempfile())
  dir.create(temp_directory)
  dir.create(paste(temp_directory,"upload",sep="/"))
  dir.create(paste(temp_directory,"corpus",sep="/"))

  cleanup <- function(){
    unlink(temp_directory, recursive = TRUE, force = TRUE)
  }

  onSessionEnded(cleanup)

  # Update filters of analysis, dependent on which values are in the database.
  languages <- uniques("language")
  genders <- uniques("gender")
  educations <- uniques("education")
  keyboards <- uniques("keyboard")
  browsers <- uniques("browser")

  updateCheckboxGroupInput(inputId = "languageCheck", choices = languages, selected = c("EN", "NL"))
  updateCheckboxGroupInput(inputId = "genderCheck", choices = genders, selected = genders)
  updateCheckboxGroupInput(inputId = "educationCheck", choices = educations, selected = educations)
  updateCheckboxGroupInput(inputId = "keyboardCheck", choices = keyboards, selected = keyboards)
  updateCheckboxGroupInput(inputId = "browserCheck", choices = browsers, selected = browsers)

  # Update filters of corpus statistics, dependent on which variables are in the session data.
  column_names <- columns("session")
  is_numeric_variable_v = Vectorize(is_numeric_variable)
  numeric_column_names <- subset(column_names, is_numeric_variable_v(column_names$columns), columns)
  updateSelectizeInput(inputId="corpusVarName", choices = column_names)
  updateSelectizeInput(inputId="xValueSelect", choices = numeric_column_names)  # TODO: only numerical values are allowed here.

  # Update the slider for the database samples.
  # !! It is possible that the database will be updated during the analysis, but this will not be visible without reloading the web page.
  # !! However, the latest samples will be different, if the slider is modified.
  count <- db_count()
  updateSliderInput(inputId = "SampleNr", value = min(2000, count), max = count)


  #  The variable that is currently selected (from session data) in the corpus analysis part.
  current_corpus_variable <- reactive({
    get_corpus_by_column(input$corpusVarName)
  })

  #  The datapoint(s) that are uploaded, filtered such that only the selected samples are there.
  uploaded_datapoint <- reactive({
    if (input$uploadType == 1 && is.null(input$Upload))
    {
        return (NULL)
    }
    if (input$uploadType == 2 && is.null(input$UploadMultiple))
    {
       return (NULL)
    }
      
    x = NULL
    # Read date from CSV files of the upload
    for(i in 1:length(input$experimentSelect))
    {
      experiment <- input$experimentSelect[i]
      table_name <- table_names()[i]
      
      path = paste(temp_directory, "upload", "data", paste(strsplit(table_name, "_")[[1]], collapse = "/"), sep="/")
      path = paste0(path, ".csv")
      
      if (!file.exists(path))
        return (NULL)
      
      if (is.null(x))  # first entry 
      {
        x = read.csv(path)[input$experimentVariableSelect]
        colnames(x) <- c(experiment)
      }
      else  # all the following entries
      {
        xx = read.csv(path)[[input$experimentVariableSelect]]
        x[experiment] <- xx
      }
    }
    
    if (input$aggregateType == 2)
    {
      col_names <- colnames(x)
      x = as.data.frame(t(apply(x, 2, mean))) # apply mean function on each column
    }
    if (input$aggregateType == 3)
    {
      x = as.data.frame(t(apply(x, 2, median))) # apply median function on each column
    }
    return(x)
  })

  # The session information of the uploaded data point.
  uploaded_session <- reactive({
    if (input$uploadType == 1 && is.null(input$Upload))
    {
      return (NULL)
    }
    if (input$uploadType == 2 && is.null(input$UploadMultiple))
    {
      return (NULL)
    }
    
    path = paste(temp_directory, "upload", "data", "session.csv", sep="/")
    if (!file.exists(path))
      return (NULL)
    
    x <- read.csv(path)

    if (input$aggregateType == 2)
    {
      xx <- data.frame(id=1)
      xx <- cbind(xx, as.data.frame(mean(x[[input$xValueSelect]]))) # apply mean function to the column
      colnames(xx) <- c("id", input$xValueSelect)
      return (xx)
    }
      
    if (input$aggregateType == 3){      
      xx <- data.frame(id=1)
      xx <- cbind(xx, as.data.frame(median(x[[input$xValueSelect]]))) # apply median function to the column
      colnames(xx) <- c("id", input$xValueSelect)
      return (xx)
    }
    x$id <- 1:nrow(x)
    return (x[c("id", input$xValueSelect)])
  })

  #  Enable / Disable the upload & aggregation fields based on which upload type is selected.
  observeEvent(input$uploadType, {
    if (input$uploadType == 1)
    {
      enable("Upload")
      disable("UploadMultiple")
      disable("aggregateType")
    }
    else
    {
      disable("Upload")
      enable("UploadMultiple")
      enable("aggregateType")
    }
  })

  #  Handle the upload and analysis of a single file.
  observeEvent(input$Upload, {
    unlink(paste(temp_directory, "/upload/*", sep=""), force = TRUE, recursive = TRUE)
    withProgress(message = "Executing analysis on the uploaded file.", {
       upload_file_to_server(input$Upload$datapath, temp_directory)})  # Ugly code, but we need to send for analysis as soon as the file is uploaded, as this is more intuitive.
  })

  #  Handle the upload and analysis of multiple files.
  observeEvent(input$UploadMultiple, {
    unlink(paste(temp_directory, "/upload/*", sep=""), force = TRUE, recursive = TRUE)
    withProgress(message = "Executing analysis on the uploaded files.", max=nrow(input$UploadMultiple),{
      setProgress(0, message = paste0("Executing analysis on the uploaded files (", 1, "/", nrow(input$UploadMultiple), ")"))
      for (i in 1:nrow(input$UploadMultiple))
      {
        filepath <- input$UploadMultiple$datapath[[i]]
        cat(paste(input$UploadMultiple$name[[i]], "\n"))
        upload_file_to_server(filepath, temp_directory)
        incProgress(amount = 1, message = paste0("Executing analysis on the uploaded files (", i, "/", nrow(input$UploadMultiple), ")"))
      }
    })
  })

  #  Get the non-filtered samples of session data (only determined by the random seed and the amount).
  non_filtered_samples <- eventReactive(c(input$resampleBtn, input$SampleNr), {
    non_filtered <- random_samples("session", input$SampleNr)
    return (non_filtered)
  })


  #  The sampled session data, with the filters applied.
  samples <- reactive({
    allow_missing <- input$includeMissingCheck
    non_filtered <- non_filtered_samples()
    filtered <- filter_by_conditions(non_filtered, "language", input$languageCheck)
    filtered <- filter_by_range(filtered, "age", input$ageSlider, allow_missing)
    filtered <- filter_by_conditions(filtered, "gender", input$genderCheck)

    filtered <- filter_by_range(filtered, "handedness", input$handednessSlider, allow_missing)
    filtered <- filter_by_range(filtered, "correctness", input$correctnessSlider, allow_missing)

    if (input$repetitionCheck != -1){
      filtered <- filter_by_conditions(filtered, "repetition", input$repetitionCheck == 1)  # convert numeric to boolean
    }
    filtered <- filtered[order(filtered$id),] # sort filtered by id.
    filtered
  })

  #  The psql table names of the current task and subtasks selected in the analysis part.
  table_names <- reactive({
    if (input$typeSelect == "component")
      return (paste("components", input$experimentSelect, sep="_"))
    if (input$typeSelect == "trial"){
      return (paste("trials", input$trialTaskSelect, input$experimentSelect, sep="_"))
    }
    name <- tolower(input$typeSelect)
    return (name = paste0(tolower(input$typeSelect), "_", lapply(input$experimentSelect, tolower)))
  })

  #  Create a comparative plot with no arguments. 
  comparative_plot_fn <- function(){
    if (input$corpusVarName == "")
      return (ggplot())  # This is needed, because the function needs to have a return value at initialization time. However, at initialization time, input$corpusVarName is empty, which would throw an error as that value can't be retrieved from the database.
    else
      return (comparative_plot(current_corpus_variable(), samples()[[input$corpusVarName]], plotType = input$statsPlotType, varName = input$corpusVarName, bins = input$corpusStatsBinSlider))
  }

  #  Create an experiment plot
  experiment_plot_fn <- function(){
    variable <- input$experimentVariableSelect

    req(length(input$experimentSelect) != 0)

    data = NULL
    for(i in 1:length(input$experimentSelect))
    {
      experiment <- input$experimentSelect[i]
      table_name <- table_names()[i]
      x <- retrieve_by_id(table_name, samples()$id)[[variable]]
      if (is.null(data) && length(x) > 0){
        data = as.data.frame(x)
        colnames(data) = c(experiment)
      }
      else{
        data[experiment] <- x
      }
    }
    req(!is.null(data))
    experiment_plot(data = data, uploaded_data = uploaded_datapoint(), plotType = input$experimentPlotSelect, variable = variable, bins = input$experimentBinSlider, 
                    xAxisVar = input$xValueSelect, sampled_session = samples(), uploaded_session = uploaded_session(), xlabel = input$typeSelect, outliers = input$allowOutliersSelect)
   }

  #  The output which shows how many samples there are selected.
  output$SampleNr <- renderUI({
    HTML(paste("<p style='display:inline'> Selected <strong><h4 style='display:inline'>", nrow(samples()), "</h4></strong> filtered samples from the corpus.</p>"))
  })

  output$isNumericCorpusVariable <- reactive({
    is_numeric_variable(input$corpusVarName)
  })


  #  The renderer of the comparative plots (pie, bar/histogram) of the filtered sample and the corpus.
  output$corpusStats <- renderPlot({
    comparative_plot_fn()
  })

  #  Download handler for the experiment plot.
  output$downloadExperimentPlot <- downloadHandler( filename = "plot.png", content = function(file){
    ggsave(file, plot=experiment_plot_fn(), height=height_fn()/50, width=width_fn()/50)
  })

  #  Download handler for the comparative plot.
  output$downloadCorpusStats <- downloadHandler( filename = "plot.png", content = function(file){
    ggsave(file, plot=comparative_plot_fn(), height=4, width=10)
  })

  output$downloadFilterCsv <- downloadHandler( filename = "dataset.zip", content = function(file){
    tables <- c()
    withProgress(message="Preparing files for download", {
      if (input$csvPerspectiveDownloadOptions == 0)
      {
        # make a list of all Perspectives and their associated tables        
        tables <- c("session", paste("components", copy_tasks, sep="_"), paste("trials", sapply(paste0("words", 1:4), paste0, "_", paste0("trial", 1:7)), sep="_"), paste("hands", hand_combinations, sep="_"),
                    paste("adjacency", c("true", "false"), sep="_"), paste("frequency", frequencies, sep="_"), paste("repetition", c("true", "false"), sep="_"))
        tables <- lapply(tables, tolower)
      }
      else
      {
        # make a list of the selected tasks & subtasks.
        tables <- c("session", table_names())
      }

      download_db <- 0 %in% input$corpusDownloadTypeCheck
      download_idfx <- 1 %in% input$corpusDownloadTypeCheck
      download_xml <- 2 %in% input$corpusDownloadTypeCheck
      download_bigram <- 3 %in% input$corpusDownloadTypeCheck

      download_unified <- input$csvDownloadOptions == 1

      removeModal()
      return(download(temp_directory, ids=samples()$id, tables=tables, file=file, idfx=download_idfx, bigram=download_bigram, analysis=download_xml, database=download_db, downloadOpt = input$userDownloadOptions, unified=download_unified))
    })
  })

  # A function that calculates the output width of the experiment plot based on different parameters.
  width_fn <- function(){
    width <- 800
    if (input$experimentPlotSelect == "Boxplot")
    {
        select_count <- length(input$experimentSelect)
        width <- min(200+100*select_count, 1500)
    }
    if (!is.null(uploaded_datapoint()) && nrow(uploaded_datapoint()) > 1)
      return (1.5*width)
    return (width)
  }

  # A function that calculates the output height of the experiment plot based on different parameters.
  height_fn <- function(){
    height <- 500
    if (input$experimentPlotSelect == "Boxplot")
      height <- 400
       
    return (height)
  }

  #  The renderer of the experiment plots (boxplot, histogram & scatterplot).
  output$experimentComparisonPlot <- renderPlot({
    experiment_plot_fn()
  },
    width = function(){
        return (width_fn())
    },
    height = function(){
        return (height_fn())
    },
    res = 100) # The default is 72 ppi.

  #  Update the slider of the bins when the corpusVarName is updated.
  observe({
    req(input$corpusVarName)
    req(is_numeric_variable(input$corpusVarName))
    x <- current_corpus_variable()
    range <- max(x)-min(x)+1
    updateSliderInput(inputId = "corpusStatsBinSlider", max = range)
  })

  #  Update the selections when selecting a new Perspective.
  observeEvent(input$typeSelect, ignoreInit=TRUE, {
    choices = c()
    lbl = input$typeSelect
    if (input$typeSelect == "component")
    {
      choices = copy_tasks
      lbl <- "Component(s)"
    }
    else if (input$typeSelect == "trial")
    {
      choices = trials
      updateSelectInput(inputId = "experimentSelect", choices = trials)
      lbl <- "Trial(s)"
    }
    else if (input$typeSelect == "Frequency")
    {
      choices = frequencies
    }
    else if (input$typeSelect == "hands")
    {
      choices = hand_combinations
      lbl <- "Hand Combination(s)"
    }
    else if (input$typeSelect == "Adjacency" || input$typeSelect == "Repetition")
    {
      choices = c("true", "false")
    }
    updateSelectInput(inputId = "experimentSelect", choices=choices, label = lbl)
  })

  # Function to generate a Download Options modal.
  downloadModal <- function()
  {
    modalDialog(title = "Download Options",
      fluidRow(
        column(width=3, checkboxGroupInput("corpusDownloadTypeCheck", "Data", choices = c("Database" = 0, "IDFX files" = 1, "Analysis XML Files" = 2, "Bigram CSV Files" = 3))),
        column(width=3, radioButtons("csvDownloadOptions", "Database Format", choices = c("Structured CSVs"=0, "Unified CSV"=1), selected = 0)),
        column(width=3, radioButtons("csvPerspectiveDownloadOptions", "Database Components", choices = c("All Components"=0, "Selected Components (see Analysis)"=1), selected = 0))      ),
      fluidRow(
        column(width=2, offset=7, downloadButton("downloadFilterCsv", "Download this data"))
      ),
      size = "m",
      footer = NULL,
      easyClose = TRUE
    )
  }
  
  # Shows the download options modal, when clicking on 'start download wizard'.
  observeEvent(input$startDownloadWizard, {
    showModal(downloadModal())
  })
  
  outputOptions(output, "isNumericCorpusVariable", suspendWhenHidden = FALSE)
}
options(shiny.fullstacktrace = TRUE)

app <- shinyApp(ui, server)