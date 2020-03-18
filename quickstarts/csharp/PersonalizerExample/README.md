# About this Quickstart
This sample interactively takes the time of day and the users's taste preference as context, and sends it to the Azure Personalizer instance, which then returns the top personalized food choice, along with the recommendation probability distribution of each food item.
The user then inputs whether or not Personalizer predicted correctly, which is data used to improve Personalizer's prediction model.

# Run sample
1. Create a Personalizer instance in the Azure portal.
2. Set environment variables PERSONALIZER_RESOURCE_KEY and PERSONALIZER_RESOURCE_ENDPOINT. These values can be found in your Cognitive Services Quick start tab in the Azure portal.
3. Build and run the sample. It will take input from the user interactively and send the data to the Personalizer instance.