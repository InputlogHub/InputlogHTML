require(ggplot2)
require(gridExtra)
require(dplyr)
require(reshape)

#  Create a histogram
#  data -> data that will be plotted
#  xlim -> limits on the x axis_max
#  breaks -> where the breaks are for each bucket
#  title -> which title should the plot have
create_histogram <- function(data, xlim, breaks, title)
{
  p <- ggplot(mapping = aes(data)) +
       geom_histogram(breaks=breaks, colour="black", fill="white") +
       coord_cartesian(xlim=xlim) +
       ggtitle(title)
  return (p)
}

#  Create a piechart
#  data -> data that will be plotted
#  title -> which title should the plot have
#  variable -> the name of the plotted variable
create_piechart <- function(data, title, variable)
{
  data <- as.data.frame(x = data)

  data <- data %>%
  group_by(data) %>%
  count() %>%
  ungroup() %>%
  mutate(per=`n`/sum(`n`)) %>%
  arrange(desc(data))
  data$label <- scales::percent(data$per, accuracy = 0.1)

  colnames(data) <- c(variable, "count", "percentage", "label")

  p <- ggplot(data, aes(x = "", y = percentage, fill = !!sym(variable))) +  # https://stackoverflow.com/a/53168593
       geom_bar(stat="identity", width = 1) +
       coord_polar("y") +
       theme_void() +
       ggtitle(title) +
       geom_text(aes(x=1, y = cumsum(percentage) - percentage/2, label=label))
  return (p)
}

#  Create a barchart
#  data -> data that will be plotted
#  title -> which title should the plot have
#  variable -> the name of the plotted variable
create_barplot <- function(data, title, variable)
{
  data <- as.data.frame(table(data))
  colnames(data) <- c(variable, "frequency")
  p <- ggplot(data, aes(x=!!sym(variable), y=frequency)) +
       geom_bar(stat = "identity") +
       ggtitle(title)

  return (p)
}

#  Create a boxplot
#  data -> data that will be plotted
#  variable -> the name of the plotted variable
#  xlabel -> the label to give to the x-axis
#  outliers -> whether to include outliers or not (and rescale the axises accordingly)
create_boxplot <- function(data, variable, xlabel = NULL, outliers = TRUE)
{
  melt_data <- melt(data, id.vars = c()) # convert a dataframe to a format of 2 columns the first column contains the task, the second the values.
  colnames(melt_data) <- c("task", variable)
  p <- ggplot()

  if (outliers){
    p <- p + geom_boxplot(melt_data, mapping = aes(x = task, y = !! sym(variable),fill=task))
  }
  else
  {
    stats <- apply(data, 2, boxplot.stats) # calculate stats for each column
    stats <- sapply(lapply(stats, '[', 1), unlist) # get the actual stats
    
    axis_min <- min(stats[1,]) # get minimum of all minimum whiskers
    axis_max <- max(stats[5,]) # get maximum of all maximum whiskers
            
    p <- p + geom_boxplot(melt_data, mapping = aes(x = task, y = !! sym(variable), fill=task), outlier.shape = NA) + 
         coord_cartesian(ylim=c(axis_min, axis_max))  # resize to tight fit window and remove outliers.
  }
  p <- p + theme(legend.position = "none") + xlab(xlabel)
  return (p)
}

#  Create a scatterplot
#  data -> data that will be plotted
#  samples -> sampled session data
#  variable -> the name of the plotted variable
#  xAxisVar -> the variable of the session data to use as X Axis
#  xlabel -> the label to give to the x-axis
create_scatterplot <- function(data, samples, variable, xAxisVar, xlabel)
{
  data[[xAxisVar]] <- samples[[xAxisVar]]
  data <- melt(data, id.var = xAxisVar)
  
  lim = c(min(data$value), max(data$value))
  colnames(data) <- c(xAxisVar, "task", variable)
  
  p <- ggplot() +
    geom_point(data, mapping = aes(x = !!sym(xAxisVar), y = !!sym(variable), col=task), alpha=0.25) +
    labs(fill=xlabel)
  #           coord_cartesian(xlim = lim, ylim = lim)  # Force the scatterplot to have a square dimension.
  
  return (p)
}

#  Create a comparative plot of the dataset and sample on a single variable
#  x -> full dataset (single variable selected)
#  xx -> sample from dataset (single variable selected)
#  plotType -> which type of plot (Pie Chart or Histogram)
#  bins -> How many bins does the histogram have
comparative_plot <- function(x, xx, plotType, varName = NULL, bins = NULL)
{
  x = na.omit(x)  # remove NA values from column
  xx = na.omit(xx) # remove NA values from column
  if (plotType == "Pie Chart")
  {
    p1 <- create_piechart(x, paste("Distribution of", varName, "in corpus"), variable = varName)
    req(length(xx) != 0)
    p2 <- create_piechart(xx, paste("Distribution of", varName, "in filtered samples"), variable = varName)
    grid.arrange(p1, p2, ncol=2)
  }
  else if (plotType == "Histogram"){
    if (is.numeric(x))
    {
      xlim <- c(min(x,xx), max(x,xx))
      breaks <- seq(min(x,xx), max(x,xx), length.out = bins + 1)

      p1 <- create_histogram(x, title=paste("Distribution of", varName, "in corpus"), xlim = xlim, breaks = breaks)
      req(length(xx) != 0)
      p2 <- create_histogram(xx, title=paste("Distribution of", varName, "in filtered samples"), xlim = xlim, breaks = breaks)
      grid.arrange(p1, p2, ncol=2)
    }
    else
    {  # TODO: use percentages.
      p1 <- create_barplot(x, paste("Distribution of", varName, "in corpus"), varName)
      req(length(xx) != 0)
      p2 <- create_barplot(xx, paste("Distribution of", varName, "in filtered samples"), varName)
      grid.arrange(p1, p2, ncol=2)
    }
  }
}

#  Create a plot for an experiment_plot
#  data -> The sampled data (different (sub)tasks) (a dataframe where each column represents a task, each row represents an observation)
#  uploaded_data -> The uploaded data (different (sub)tasks)
#  plotType -> which type of plot (Boxplot or Scatterplot)
#  variable -> the name of the variable
#  bins -> How many bins does the histogram have
#  xAxisVar -> The session variable against which we will plot the other variable
#  sampled_session -> The sampled session data
#  uploaded_session -> The uploaded session data
#  xlabel -> The label for the x-axis
#  outliers -> whether to include outliers in a boxplot
experiment_plot <- function(data, uploaded_data, plotType, variable, bins = NULL, xAxisVar = NULL, sampled_session = NULL, uploaded_session = NULL, xlabel = NULL, outliers = TRUE)
{
  if (plotType == "Boxplot")
  {
    p <- create_boxplot(data, variable, xlabel, outliers)
    
    if (is.null(uploaded_data))
      return (p)
    
    if (nrow(uploaded_data) > 1)
    {
      p2 <- create_boxplot(uploaded_data, variable, xlabel, outliers)
      return (grid.arrange(p, p2, ncol=2))
      
    }
    # Add datapoint(s) to visualisation.
    melt_uploaded_data <- melt(uploaded_data, id.vars = c()) # convert a dataframe to a format of 2 columns the first column contains the task, the second the values.
    colnames(melt_uploaded_data) <- c("task", variable)
    
    p <- p + geom_point(melt_uploaded_data, mapping=aes(x = task, y = !! sym(variable), fill = task), shape=21, colour="black", size=3, stroke=2)
    return (p)
  }
#     else if (plotType == "Histogram")
#     {
#       colours <- rainbow(n=select_count, alpha = 0.25)
#       dark_colours <- rainbow(n=select_count, alpha=0.65)
#       max_y_range <- 0
#       bins <- seq(min(data), max(data), length.out = bins + 1)
#
#
#       # VERY UGLY CODE, but we need to calculate this twice, first to know the ylimit, then to plot them.
#       # TODO: maybe rewrite this to be clearer.
#       for (j in 1:2){
#         for (i in 1:select_count)
#         {
#           col = colnames(data)[i]
#           plt <- hist(data[[col]], breaks=bins, plot = FALSE)
#           max_y_range <- max(max_y_range, max(plt$counts))
#
#           if (j == 1)  # first iteration, we do not want to plot the histograms yet.
#             next
#
#           current_colours <- rep(colours[i], length(bins))
#           if (length(pts) != 0){
#             current_colours[Position(function(x) x > pts[i], bins) - 1] <- dark_colours[i]
#           }
#           plot(plt, col = current_colours, main = "", xlab = variable, ylim = c(0,max_y_range), add = (i != 1))  # The first one is the basis, all the others are added on the canvas.
#         }
#         legend("bottomright", legend = colnames(data), col = colours, pch = 15)
#       }
#     }
    else if (plotType == "Scatterplot")
    {
      p <- create_scatterplot(data, sampled_session, variable, xAxisVar, xlabel)
      
      if (is.null(uploaded_data))
        return (p)
      
      if (nrow(uploaded_data) > 1)
      {
        p2 <- create_scatterplot(uploaded_data, uploaded_session, variable, xAxisVar, xlabel)
        return (grid.arrange(p, p2, ncol=2))
      }
      uploaded_data[[xAxisVar]] <- uploaded_session[[xAxisVar]]
      uploaded_data <- melt(uploaded_data, id.var = xAxisVar)
      
      colnames(uploaded_data) <- c(xAxisVar, "task", variable)
      
      p <- p +
        geom_point(uploaded_data, mapping = aes(x = !!sym(xAxisVar), y = !!sym(variable), fill=task), shape=21, colour="black", size=2, stroke=2) + labs(fill="Uploaded datapoint")
      return (p)
    }
}
