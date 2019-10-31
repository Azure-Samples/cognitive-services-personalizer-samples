# Real World Reinforcement Learning Workshop

## Abstract
Microsoft recently announced the Azure Cognitive Service, Personalizer, aimed at democratizing real world reinforcement learning for content personalization. Its goal is to make reinforcement learning accessible to everyone, not just machine learning experts. Personalizer is the result of a successful partnership between Microsoft Research and Azure Cognitive Services aimed at rapid technology transfer and innovation.

In this workshop you will learn the theory behind contextual bandits and how this applies to content personalization. We will walk you through setting up the service, writing your first application, and optimizing the policy using offline optimization.

## Workshop Instructions
1. [Create free Azure/Microsoft account](https://azure.microsoft.com/en-us/free/)
2. Provision free instance of Personalizer
    1. Go to [Azure Portal](https://portal.azure.com)
    2. Search for "Cogitive Services"
    3. On Cognitive Services page click "Add"
    4. Search for "Personalizer" and click "Create"
3. Install Python Client
   ```pip install azure.cognitiveservices.personalizer```
3. Paste your endpoint into [line 34 of ./demo/demo.py](https://github.com/Azure-Samples/cognitive-services-personalizer-samples/blob/master/demos/workshop-demo/demo.py#L34)
4. Paste your key into [line 35 of ./demo/demo.py](https://github.com/Azure-Samples/cognitive-services-personalizer-samples/blob/master/demos/workshop-demo/demo.py#L35)
5. run `python ./demo/demo.py`

## Next steps
- [Personalizer](https://azure.microsoft.com/en-us/services/cognitive-services/personalizer/)
- [Personalizer docs](https://docs.microsoft.com/en-us/azure/cognitive-services/personalizer/)
- How can personalization be integrated into what you work on day to day?
