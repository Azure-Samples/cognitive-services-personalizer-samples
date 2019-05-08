using System;
using System.Collections.Generic;

namespace PersonalizerBuild1
{
    public class UserSimulator
    {

        int simulatedUserId;
        Random rand;

        string[] possibleActionIds = new string[] { "pasta", "ice_cream", "juice", "salad" };

        private new Dictionary<string, string>  responses = new Dictionary<string, string>();



        public UserSimulator(int simulatedUserId, Random rand)
        {
            this.simulatedUserId = simulatedUserId;
            this.rand = rand;

            InitializeResponses();
        }



        public string ReturnSimulatedAction(IList<object> context)
        {

            return GetResponse((FoodContext)context[0]);
        }

        public string GetResponse(FoodContext ctx)
        {
            string key = GetKey(ctx);
            if (responses.ContainsKey(key))
            {
                return responses[key];
            }
            else
            {
                return GetRandomResponse();
            }


        }

        public void InitializeResponses()
        {
            //{ "morning","afternoon","evening"  };
            //{ "sunny", "cloudy", "rainy" };
            //pasta, ice_cream, juice, salad

            AddSimResponse(1, "morning", "sunny", "juice");
            AddSimResponse(1, "morning", "cloudy", "juice");
            AddSimResponse(1, "morning", "rainy", "juice");

            AddSimResponse(1, "afternoon", "sunny", "ice_cream");
            AddSimResponse(1, "afternoon", "cloudy", "juice");
            AddSimResponse(1, "afternoon", "rainy", "salad");

            AddSimResponse(1, "evening", "sunny", "pasta");
            AddSimResponse(1, "evening", "cloudy", "pasta");
            AddSimResponse(1, "evening", "rainy", "pasta");


        }

        public string GetRandomResponse()
        {
            return possibleActionIds[rand.Next(possibleActionIds.Length)];
        }

        public void AddSimResponse(int userId, string time, string weather, string foodChoice)
        {
            responses.Add(GetKey(userId, time, weather), foodChoice);

        }

        public string GetKey(int userId, string time, string weather)
        {
            return userId.ToString() + "_" + time + "_" + weather;
        }

        public string GetKey (FoodContext ctx)
        {
            return GetKey(ctx.userid, ctx.time, ctx.weather); 
        }

    }
}
