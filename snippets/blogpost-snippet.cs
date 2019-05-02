           

using System;
using System.Collections;


namespace Snippet
    {

    class SomeClass
    {
        public string getBestNewsArticleID()
        {
                
            // Create context to personalize for, from user data or contextual information
            IList<object> currentContext = new List<object>() {
                    new { time = "EarlyEvening" },
                    new { weather = "Rainy" },
                    new { userFavoriteTopic = "SpaceExploration" },
                    new { userProfile = "PayingUser" }
                    //Add your own features in your own schema
            };


            // Provide a list of possible actions -articles- to choose from
            var actionsB = new RankableAction[] {
                new RankableAction("news_article_1", new List<object>{ new { 
                    topic = "politics", 
                    breakingNews = true,
                    controversy= false }}),
                new RankableAction("news_article_2", new List<object>{ new { 
                    topic = "technology", 
                    breakingNews = false,
                    publishedWithin = "week"}})
            };

            var request = new RankRequest(actions, currentContext, null, eventId);                

            // Call Personalizer and get the action that should be shown
            RankResponse response = personalizer.Rank(request);

            //Use the result given by Personalizer
            return response.RewardActionId;
        }

        //Later on, we send the reward for the event.
        Void UserReadNewsEvent(float percentageScrolled)
        {
            // Send the reward for the action based on a KPI or goal.
            //in this example, great personalization means the user scrolled to the end.
            personalizer.Reward(response.EventId, new RewardRequest(percentageScrolled));
        }



    }
}



           
