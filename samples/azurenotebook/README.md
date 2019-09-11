# Cognitive Services Personalizer notebook

This sample notebook ceate 10,000 Rank events with followup Reward calls. The user and context features are randomly choosen while the action features is a static list. The sampling happens over a period of time so that the Personalizer training frequency can use the previous sampling period to learn from and improve the next sampling period.

## To use this sample

1. Create a Personalizer resource in the [Azure portal](https://azure.microsoft.com/free/). 
1. While still in the Azure portal, on your new resource for personalizer, select ** Quick start**, then copy your endpoint and key. You will need these in the next step.
1. While still in the Azure portal, on your new resource for personalizer, select **Settings**, then change the **Model update frequency** value to be 1 minute. Save the change. 
1. In this notebook, in the `personalization.ipynb` file, change the values for `personalizer_endpoint` and `personalizer_key`. 
1. Run the notebook. The loop at the bottom of the notebook prints statements so you can watch the iterations. Running the iterations will take a few minutes.
1. When the loop iterations are done, run the final block that creates the chart. 

## What the sample shows about Personalizer

The sample demonstrates several features of Personalizer:

1. How to use the Python SDK. 
1. How to send data to the Rank and Reward APIs.
1. How to use Personalizer to create a data set able to help with offline evaluations.
1. How to visualize the increased positive behavior from using Personalizer.