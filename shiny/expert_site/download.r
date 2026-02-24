source("rest.r")
require("zip")

corpus_directory <- "C:\\Users\\siebe\\OneDrive\\Documenten\\+Universiteit\\inputlog\\2026\\shiny\\database\\corpus"

downloadDBUnified <- function(folder, tables, ids)
{
  data <- NULL
  for (table_name in tables)
  {
    sampled <- retrieve_by_id(table_name, ids)
    sampled <- subset(sampled, select = -c(id))
    
    if (table_name != "session")
      colnames(sampled) <- paste0(table_name, "_", colnames(sampled))
    
    if (is.null(data))
      data <- sampled
     else{
      data <- cbind(data, sampled)
     }
  }
  ids <- data.frame(ids)
  colnames(ids) <- "id"
  data <- cbind(ids, data)
  
  if (!dir.exists(folder))
  {
    dir.create(folder, recursive = TRUE)
  }
  
  write.csv(data, paste(folder, "data.csv", sep="\\\\"), row.names = FALSE)
}

downloadDBStructured <- function(folder, tables, ids)
{
  for(i in 1:length(tables))
  {
    file_location = strsplit(tables[[i]], "_")[[1]]
    file_name = file_location[length(file_location)]
    file_location = paste(file_location[1:length(file_location)-1], collapse="\\\\")
    
    file_location = paste(folder, file_location, sep="\\\\")
    table_name = tables[[i]]

    if (!dir.exists(file_location))
    {
      dir.create(file_location, recursive = TRUE)
    }
    setwd(file_location)
    sampled <- retrieve_by_id(table_name, ids)
    file_name <- paste0(file_name,".csv")
    write.csv(sampled, file_name, row.names = FALSE)
  }
}

downloadIDFX <- function(folder, ids)
{  # Create IDFX structuur 
  orig_filenames <- paste0(corpus_directory, "/idfx/", ids, ".idfx")
  new_filenames <- paste0(folder, "/", ids, ".idfx")
  file.copy(orig_filenames, new_filenames)
}

downloadBigram <- function(folder, ids)
{  # Create bigram structuur
  orig_filenames <- paste0(corpus_directory, "/bigrams/", ids, "_bigram.csv")
  new_filenames <- paste0(folder, "/", ids, "_bigram.csv")
  file.copy(orig_filenames, new_filenames)
}

downloadAnalysis <- function(folder, ids)
{  # Create XML structuur
  orig_filenames <- paste0(corpus_directory, "/xml/", ids, ".xml")
  new_filenames <- paste0(folder, "/", ids, ".xml")
  file.copy(orig_filenames, new_filenames)
}

# downloadOpt = 0 -> only corpus, 1 -> only user, 2 -> both
download <- function(folder, unified = FALSE, idfx = FALSE, bigram = FALSE, analysis = FALSE, database = TRUE, tables = NULL, ids = NULL, file=NULL, downloadOpt = 0)
{
  corpus_folder <- paste(folder, "corpus", sep="\\\\")
  upload_folder <- paste(folder, "upload", sep="\\\\")
  user_folder <- paste(folder, "user", sep="\\\\")
  if (!dir.exists(user_folder))
    dir.create(user_folder)
  
  download_db <- downloadOpt == 0 || downloadOpt == 2
  download_user <- (downloadOpt == 1 || downloadOpt == 2) && dir.exists(upload_folder)
  
  
  if (database){
    if (download_db)
    {
      db_folder = paste(corpus_folder, "data", sep="\\\\")
      if (unified)
      {
          downloadDBUnified(folder=db_folder, tables, ids)
      }
      else
      {
          downloadDBStructured(folder=db_folder, tables, ids)
      }
    }
    if (download_user)
    {
      data_user_folder = paste(user_folder, "data", sep="\\\\")
      dir.create(data_user_folder)
      file.copy(paste(upload_folder, "data", sep="\\\\"), data_user_folder, recursive=TRUE)
    }
  }

  if (idfx)
  {
    if (download_db)
    {
      idfx_folder = paste(corpus_folder, "idfx", sep="/")
      dir.create(idfx_folder)
      downloadIDFX(folder=idfx_folder, ids)
    }
    if (download_user)
    {
      idfx_user_folder = paste(user_folder, "idfx", sep="/")
      dir.create(idfx_user_folder)
      file.copy(paste(upload_folder, "idfx", sep="/"), idfx_user_folder, recursive=TRUE)
    }
  }

  if (bigram)
  {
    if (download_db)
    {
      bigram_folder = paste(corpus_folder, "bigram", sep="/")
      dir.create(bigram_folder)
      downloadBigram(folder=bigram_folder, ids)
    }
    if (download_user)
    {
      bigram_user_folder = paste(user_folder, "bigram", sep="/")
      dir.create(bigram_user_folder)
      file.copy(paste(upload_folder, "bigrams", sep="/"), bigram_user_folder, recursive=TRUE)
    }
  }

  if (analysis)
  {
    if (download_db)
    {
      analysis_folder = paste(corpus_folder, "analysis", sep="/")
      dir.create(analysis_folder)
      downloadAnalysis(folder=analysis_folder, ids)
    }
    if (download_user)
    {
      analysis_user_folder = paste(user_folder, "analysis", sep="/")
      dir.create(analysis_user_folder)
      file.copy(paste(upload_folder, "xml", sep="/"), analysis_user_folder, recursive=TRUE)
    }
  }
  
  owd <- setwd(folder)
  
  files2zip <- c(dir("corpus", recursive = TRUE, full.names = TRUE), dir("user", recursive = TRUE, full.names = TRUE))
  setwd(owd)
  zipped <- zip::zip(file, files = files2zip, root=folder)
  unlink(paste0(corpus_folder, "/*"), recursive=TRUE, force=TRUE)
  unlink(paste0(user_folder, "/*"), recursive=TRUE, force=TRUE)
  return (zipped)
}
