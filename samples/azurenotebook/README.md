# Personalizer simulation in an Azure notebook

This tutorial simulates a Personalizer loop _system_ which suggests which type of coffee a customer should order. The users and their preferences are stored in a [user dataset](usrs.json). Information about the coffee is also available and stored in a [coffee dataset](coffee.json).

This sample makes 4000 requests. Each request chooses a random user, time of day, and weather. Then sends these values along with the entire list of coffee choices to Personalizer. Personlizer recommends a selection, which the system checks against the user's known preferences. 

If Personalizer recommended correctly, the reward of 1 is sent back to Personalizer. Otherwise the value of 0 is sent back to Personalizer. The Personalizer loop automatically retrains at a rate specified in the Azure portal with the name of **update model frequency** on the **Settings** page. 

When the model is retrained, the Personalizer loop should be more successful with the recommendations.

## Prerequisites

* [Azure notebooks](https://notebooks.azure.com/) account
* Personalizer loop configured with a 5 minute model update frequency

## How to use this sample

1. Create a new Azure notebook project.
1. Upload the files in this directory to the Azure notebook project. 
1. Open the Personalizer.ipynb file and change the following values:

    * The value for `<your-resource-name>` in the `personalization_base_url` to the value for your Personalizer resource
    * The value for `subscription_key` variable to one of the Personalizer resource keys. 

1. Run each cell from top to bottom. Wait until each cell is complete before running the next cell. 
1. At the beginning of the system, the last updated time is displayed, then every 500 requests. This is important so you can see and know when the retraining happened. 
1. After the request loop completes, run the chart to see where the Personalizer service began to perform with more accurate suggestions. That point in the chart should roughly correlate to the model update frequency change. 

    If your loop doesn't finish with a performance percentage between 70 and 80 percent, perform an offline evaluation to find a better learning policy. 
