# Big Data Module

## Overview

This coursework focuses on **parallelisation and scalability in the cloud** using **PySpark** and **TensorFlow/Keras**. 

The main objectives are to:

- Parallelise data preprocessing tasks with Spark.
- Measure cloud performance for different data processing configurations.
- Analyse and evaluate results.
- Discuss theoretical implications of cloud-based data processing.

The work is structured into three main sections:

1. **Data Preprocessing**  
   - Preprocess a dataset of flower images (3600 images, 5 classes).  
   - Re-implement TensorFlow preprocessing in Spark to take advantage of parallelisation.  
   - Write processed data to the cloud using TFRecord files.  

2. **Performance Measurement**  
   - Measure data reading speeds for different parameters in the cloud.  
   - Parallelise speed tests with Spark to evaluate efficiency and throughput.  
   - Analyse results, perform linear regression, and compare cloud vs. single-machine performance.  

3. **Theoretical Discussion**  
   - Relate results to cloud configuration optimisation concepts (Cherrypick, Alipourfard et al., 2017).  
   - Discuss strategies for different workloads (batch vs. streaming).  
   - Provide insights on practical implications for large-scale machine learning in the cloud.  

## Project Structure

- **Notebook:** Main `.ipynb` file with coding tasks, comments, and outputs.  
- **Report:** PDF document containing theoretical answers, analysis, tables, and screenshots from Google Cloud.  
- **Scripts:** PySpark scripts for preprocessing and performance tests (e.g., `spark_job.py`).  
- **Cloud Resources:** Storage buckets and Dataproc clusters for running parallelised tasks.  

## Running the Project

### Local Development

- Use Google Colab or a local Jupyter notebook.  
- Install Spark locally for testing before deploying to the cloud.  
- Mount Google Drive for persistent storage and create a project directory.  

### Cloud Execution

- Set up a Google Cloud project with **Dataproc** and **Storage** APIs enabled.  
- Create a storage bucket to hold processed data and results.  
- Deploy PySpark jobs on Dataproc clusters (single-node and multi-node) to run preprocessing and speed tests.  
- Collect performance metrics from the Dataproc web interface for analysis.  

## Notes

- Code includes clear comments and explanations.  
- For cloud tasks, performance data for reporting and analysis is recorded.  
- Focus is on understanding **high-level workflow, parallelisation, and scalability**, rather than low-level TensorFlow internals.  

