'use strict';
const uuidv1 = require('uuid/v1');
const Personalizer = require('@azure/cognitiveservices-personalizer');
const CognitiveServicesCredentials = require('ms-rest-azure').CognitiveServicesCredentials;
const readline = require('readline-sync');

async function main() {

  // The key specific to your personalization service instance; e.g. "0123456789abcdef0123456789ABCDEF"
  let serviceKey = "24df52132b8946f5abf248b1e4e66760";

  // The endpoint specific to your personalization service instance; e.g. https://westus2.api.cognitive.microsoft.com
  let baseUri = "https://westus2.api.cognitive.microsoft.com/";

  let credentials = new CognitiveServicesCredentials(serviceKey);

  // Initialize Personalization client.
  let personalizerClient = new Personalizer.PersonalizerClient(credentials, baseUri);

  let runLoop = true;



  do {

    // this isn't correct - it looks like it is off of mappers instead of models
    let rankRequest = new Personalizer.PersonalizerModels.RankRequest();

    const eventId= uuidv1();
    let contextFeatures= getContextFeaturesList();
    let actions= getActionsList();
    let excludedActions= getExcludedActionsList();

    // Create a rank request.
    /*
    let rankRequest = {
      eventId,
      contextFeatures,
      actions,
      excludedActions,
      deferActivation: false
    };
    */

    console.log(JSON.stringify(rankRequest));

    // Rank the actions
    let rankResponse = await personalizerClient.rank(rankRequest,{});

    console.log("\nPersonalization service thinks you would like to have:\n")
    console.log(rankResponse.rewardActionId);

    // Display top choice to user, user agrees or disagrees with top choice
    let reward = getReward();

    console.log("\nPersonalization service ranked the actions with the probabilities as below:\n");
    for (var i = 0; i < rankResponse.ranking.length; i++) {
      console.log(JSON.stringify(rankResponse.ranking[i]) + "\n");
    }

    // Send the reward for the action based on user response.
    let rewardRequest = Personalizer.PersonalizerModels.RewardRequest = {
      value: reward
    };

    await personalizerClient.reward(rankRequest.eventId, { reward: rewardRequest });

    runLoop = continueLoop();

  } while (runLoop);
}

function continueLoop() {
  var answer = readline.question("\nPress q to break, any other key to continue.\n")
  if (answer.toLowerCase() === 'q') {
    return false;
  }
  return true;
}

function getReward() {
  var answer = readline.question("\nIs this correct (y/n)\n");
  if (answer.toLowerCase() === 'y') {
    console.log("\nGreat| Enjoy your food.");
    return 1;
  }
  console.log("\nYou didn't like the recommended food choice.");
  return 0;
}

function getContextFeaturesList() {
  var timeOfDayFeatures = ['morning', 'afternoon', 'evening', 'night'];
  var tasteFeatures = ['salty', 'sweet'];

  var answer = readline.question("\nWhat time of day is it (enter number)? 1. morning 2. afternoon 3. evening 4. night\n");
  var selection = parseInt(answer);
  var timeOfDay = selection >= 1 && selection <= 4 ? timeOfDayFeatures[selection - 1] : timeOfDayFeatures[0];

  answer = readline.question("\nWhat type of food would you prefer (enter number)? 1. salty 2. sweet\n");
  selection = parseInt(answer);
  var taste = selection >= 1 && selection <= 2 ? tasteFeatures[selection - 1] : tasteFeatures[0];

  console.log("Selected features:\n");
  console.log("Time of day: " + timeOfDay + "\n");
  console.log("Taste: " + taste + "\n");

  return [
    {
      "time": timeOfDay
    },
    {
      "taste": taste
    }
  ];
}

function getExcludedActionsList() {
  return [
    "juice"
  ];
}

function getActionsList() {
  return [
    {
      "id": "pasta",
      "features": [
        {
          "taste": "salty",
          "spiceLevel": "medium"
        },
        {
          "nutritionLevel": 5,
          "cuisine": "italian"
        }
      ]
    },
    {
      "id": "ice cream",
      "features": [
        {
          "taste": "sweet",
          "spiceLevel": "none"
        },
        {
          "nutritionalLevel": 2
        }
      ]
    },
    {
      "id": "juice",
      "features": [
        {
          "taste": "sweet",
          "spiceLevel": "none"
        },
        {
          "nutritionLevel": 5
        },
        {
          "drink": true
        }
      ]
    },
    {
      "id": "salad",
      "features": [
        {
          "taste": "salty",
          "spiceLevel": "low"
        },
        {
          "nutritionLevel": 8
        }
      ]
    }
  ];
}

var program = main()
.then(result => console.log("done"))
.catch(err=> console.log(err));
