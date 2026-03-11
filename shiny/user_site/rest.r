require(httr)
require(jsonlite)

port <- 5000
analysis_port <- 4343

base_db_url = paste0("http://127.0.0.1:", port, "/")
base_analysis_url = paste0("localhost:",analysis_port,"/")

REST_get <- function(url)
{
  print("URL")
  print(url)
  return( as.data.frame(
    fromJSON(
      content(
        GET(url, type="basic")
        , "text", encoding="UTF-8"
      )
      , flatten = TRUE
    )
  ))
}

REST_post_json <- function(url, label, data)
{
  json <- paste('{"', label, '": ', toJSON(data), "}", sep="")
  return( as.data.frame(
    fromJSON(
      content(
        POST(url, content_type("application/json"), body = json)
        , "text", encoding="UTF-8"
      )
      , flatten = TRUE
    )
  ))
}

REST_post_file <- function(url, file_path, user_path)
{
  POST(url, content_type("multipart/form-data"), body = c(upload_file(file_path), "user_path"=user_path))
}

uniques <- function(column_name)
{
  endpoint <- "uniques/session/"
  url <- paste(base_db_url, endpoint, column_name, sep="")

  get_uniques <- REST_get(url)
  return (sort(get_uniques$uniques))
}

percentile <-function(value)
{
  endpoint <- "percentile/session/score/"
  url <- paste(base_db_url, endpoint, value, sep="")
  return (REST_get(url)$percentile)
}

columns <- function(table_name)
{
  url <- paste(base_db_url, "columns/", table_name, sep="")

  get_columns <- REST_get(url)
  return (get_columns)
}

get_corpus_by_column <- function(column_name)
{
  url <- paste(base_db_url, "get/session/column/", column_name, sep="")
  return (REST_get(url)[[column_name]])
}

latest_samples <- function(table_name, amount)
{
  url <- paste(base_db_url, "get/", table_name, "/latest/", amount, sep="")
  return (REST_get(url))
}

random_samples <- function(table_name, amount)
{
  url <- paste(base_db_url, "get/", table_name, "/random/", amount, sep="")
  return (REST_get(url))
}

db_count <- function()
{
  url <- paste(base_db_url, "count", sep="")
  return (REST_get(url)$count)
}

retrieve_by_id <- function(table_name, ids)
{
  url <- paste(base_db_url, "get/", table_name, "/byid", sep="")
  return (REST_post_json(url, "ids", ids))
}

upload_file_to_server <- function(idfx_path, user_path)
{
  url <- paste0(base_analysis_url, "upload/")
  return (REST_post_file(url, idfx_path, user_path))
}

is_numeric_variable <- function(var)
{
  frame <- retrieve_by_id("session", c(1))
  return (is.numeric(frame[[var]]))
}
