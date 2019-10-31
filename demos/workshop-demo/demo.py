from azure.cognitiveservices.personalizer import PersonalizerClient
import azure.cognitiveservices.personalizer.models as models
from msrest.authentication import CognitiveServicesCredentials
from helpers import SlidingAverage

from datetime import datetime
from random import randint

def run(user_preference, actions, client, duration_sec):
    start = datetime.now()
    ctr = SlidingAverage(window_size = 20)
    while (datetime.now() - start).total_seconds() < duration_sec:
        index = randint(0, len(user_preference) - 1)
        
        user = user_preference[index][0]
        preference = user_preference[index][1]

        request=models.RankRequest(
            context_features=user,
            actions=actions
        )
        response=client.rank(request)
        reward = 1.0 if response.reward_action_id==preference else 0.0
        client.events.reward(event_id=response.event_id, value=reward)

        ctr.update(reward)
        print('CTR: ' + str(ctr.get()))
        for action in response.ranking:
            print(action.id + ': ' + str(action.probability))


def main():

    client = PersonalizerClient(endpoint="", # Put your endpoint here
        credentials=CognitiveServicesCredentials(""))    # Put your credentials here

    #Available content
    actions=[
        models.RankableAction(
            id='politics',
            features=[{'topic': 'politics'}]),
        models.RankableAction(
            id='sports',
            features=[{'topic': 'sports'}]),
        models.RankableAction(
            id='music',
            features=[{'topic': 'music'}]
        )]

    #User features
    Tom = {'name': 'Tom'}
    Anna = {'name': 'Anna'}

    #Time features
    Monday = {'day': 'Monday'}
    Sunday = {'day': 'Sunday'}

    context = [Tom, Monday]

    request=models.RankRequest(
        context_features=context,
        actions=actions
    )

    response=client.rank(request)
    # Show content to user, evaluate and provide reward back to service
    reward = 1.0
    client.events.reward(event_id=response.event_id, value=reward)

    # Since we are doing cold start and there is no model, all probabilities are the same
    for action in response.ranking:
        print(action.id + ': ' + str(action.probability))


    # Tom and Anna have certain preferences what to read on Monday and Sunday
    scenario = [([Tom, Monday], 'politics'),
                ([Tom, Sunday], 'music'),
                ([Anna, Monday], 'sports'),
                ([Anna, Sunday], 'politics')]

    run(scenario, actions, client, 5400)

    # Olympics started and both Tom and Anna are following sport news during weekend
    scenario = [([Tom, Monday], 'politics'),
                ([Tom, Sunday], 'sports'),
                ([Anna, Monday], 'sports'),
                ([Anna, Sunday], 'sports')]
    run(scenario, actions, client, 5400)

if __name__== '__main__':
  main()
