using System;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Personalizer;
using Microsoft.Azure.CognitiveServices.Personalizer.Models;

namespace PersonalizerBuild1
{
    public class Simulator
    {
        string[] timeContextValues = new string[] { "morning","afternoon","evening"  };
        string[] weatherContextValues = new string[] { "sunny", "cloudy", "rainy" };

        private Random rand = new Random(DateTime.Now.Millisecond);

        PersonalizerClient client;

        public Simulator(PersonalizerClient client)
        {
            this.client = client;
        }

        public void SimulateEvents(int n)
        {
            float sumRewards = 0;

            for(int i = 0; i<n; i++)
            {
                Console.Write(i.ToString()+":" );
                sumRewards = sumRewards + SimulateEvent();

                Console.Write("[{0}]", sumRewards/(i+2));

            }
        }

        public float SimulateEvent()
        {

            IList<RankableAction> actions = GetActions();

            int userId = 1;

            UserSimulator sim = new UserSimulator(userId, rand);

            var currentContext = GetSimulatedContext(userId);

            string eventId = Guid.NewGuid().ToString();

            var request = new RankRequest(actions, currentContext, null, eventId);
            RankResponse response = client.Rank(request);

            //RankResponse response = new RankResponse();

            float reward = 0f;

            string simulationResponse = sim.ReturnSimulatedAction(currentContext);

            Console.WriteLine("For Context {2}: Personalizer suggested {0}, simulation chose {1}",response.RewardActionId, simulationResponse, sim.GetKey((FoodContext)currentContext[0]));

            if (response.RewardActionId ==  simulationResponse)
            {
                reward = 1f;
            }

            // Send the reward for the action based on user response.
            client.Reward(response.EventId, new RewardRequest(reward));

            return reward;
        }


        public IList<object> GetSimulatedContext(int userId)
        {
            IList<object> currentContext = new List<object>() {
                    new FoodContext {
                        time= timeContextValues[rand.Next(3)], 
                        weather=weatherContextValues[rand.Next(3)], 
                        userid = userId
                        }

               
                };


            return currentContext;

        }



        public IList<RankableAction> GetActions()
        {
            IList<RankableAction> actions = new List<RankableAction>
            {
                new RankableAction
                {
                    Id = "pasta",
                    Features =
                    new List<object>() { new {id="pasta" }, new { taste = "salty", spiceLevel = "medium" }, new { nutritionLevel = 5, cuisine = "italian" } }
                },

                new RankableAction
                {
                    Id = "ice_cream",
                    Features =
                    new List<object>() { new { id = "ice_cream" }, new { taste = "sweet", spiceLevel = "none" }, new { nutritionalLevel = 2 } }
                },

                new RankableAction
                {
                    Id = "juice",
                    Features =
                    new List<object>() { new { id = "juice" }, new { taste = "sweet", spiceLevel = "none" }, new { nutritionLevel = 5 }, new { drink = true } }
                },

                new RankableAction
                {
                    Id = "salad",
                    Features =
                    new List<object>() { new { id = "salad" }, new { taste = "salty", spiceLevel = "low" }, new { nutritionLevel = 8 } }
                }
            };

            return actions;
        }

    }

    public class FoodContext
    {
        public string time;
        public string weather;
        public int userid;
    }
}
