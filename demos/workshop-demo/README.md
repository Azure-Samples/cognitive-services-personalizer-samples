# Real World Reinforcement Learning Workshop

## Abstract
Microsoft recently announced the Azure Cognitive Service, Personalizer, aimed at democratizing real world reinforcement learning for content personalization. Its goal is to make reinforcement learning accessible to everyone, not just machine learning experts. Personalizer is the result of a successful partnership between Microsoft Research and Azure Cognitive Services aimed at rapid technology transfer and innovation.

In this workshop you will learn the theory behind contextual bandits and how this applies to content personalization. We will walk you through setting up the service, writing your first application, and optimizing the policy using offline optimization.

## Workshop Instructions
1. Provision a free instance of Personalizer
    1. Go to [Azure Portal](https://portal.azure.com)
    1. Click "Create a resource"
    1. Search the market place for "**Cognitive Services Personalizer**" or "**Personalizer**" (make sure it's not the one that only says 'Cognitive Services')
    1. Click the "Personalizer" panel
    1. Click the "Create" button
    1. Fill out the form and click "Create"
1. Install Python Client
   ```pip install azure.cognitiveservices.personalizer```
1. Paste your endpoint into [line 34 of ./demo/demo.py](https://github.com/Azure-Samples/cognitive-services-personalizer-samples/blob/master/demos/workshop-demo/demo.py#L34)
1. Paste your key into [line 35 of ./demo/demo.py](https://github.com/Azure-Samples/cognitive-services-personalizer-samples/blob/master/demos/workshop-demo/demo.py#L35)
1. Change your Personalizer settings
    1. Go to your provisioned Personalizer instance
    1. Click the "Configuration" tab
    1. Change the "Reward wait time" to 5 seconds
    1. Change the "Model update frequency" to 30 seconds
    1. Click "Save"
1. Run `python ./demo/demo.py`

## Next steps
- [Personalizer](https://azure.microsoft.com/en-us/services/cognitive-services/personalizer/)
- [Personalizer docs](https://docs.microsoft.com/en-us/azure/cognitive-services/personalizer/)
- How can personalization be integrated into what you work on day to day?
