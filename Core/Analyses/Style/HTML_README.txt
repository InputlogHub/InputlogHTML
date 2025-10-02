OVERVIEW
The reporting pipeline has been enhanced as of July 16, 2025, to generate HTML reports for immediate review, while retaining original XML outputs for auditing or specialized XML workflows. Users can continue to access the legacy Analysis.xml files in the Xml folder. The output directory holds newly generated HTML reports. 

VIEWING HTML REPORTS
Navigate to the output folder and open any .html file by double-clicking it in your file explorer or by selecting it from within your editor. Viewing HTML reports requires only a modern web browser such as Chrome, Firefox, or Edge.

LEGACY XML VISUALIZATION (OPTIONAL)
To serve XML files over HTTP, open a terminal in the project root or output folder and run the command python -m http.server 8000. Then point your browser to http://localhost:8000/Xml/<YOUR_FILE_NAME>.xml, replacing <YOUR_FILE_NAME> with the specific filename you wish to view. Alternatively, Microsoft Edge users may enable the Cross-Origin Resource Sharing flag in the browser settings; note that this feature is deprecated and may be removed in future versions.

NOTES ON RECENT CHANGES
As of July 16, 2025, XML visualizations have been transitioned to HTML by default. No additional software is required to view reports beyond a standard browser. Python is only necessary if you opt to host XML outputs locally.

SUPPORT
For questions, issues, or contributions, please open an issue or submit a pull request in the project repository.