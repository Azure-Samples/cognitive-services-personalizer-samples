// You need to add the following client libraries to your project:
// client-runtime-1.6.6.jar - https://search.maven.org/search?q=g:com.microsoft.rest

import java.util.List;
import java.util.Scanner;

import com.microsoft.personalizer.models.*;
import com.microsoft.rest.RestClient;
import com.microsoft.rest.ServiceResponseBuilder.Factory;
import com.microsoft.rest.serializer.JacksonAdapter;
import okhttp3.Request;
import com.microsoft.personalizer.implementation.PersonalizationClientImpl;

public class PersonalizerQuickstart {

    // The key specific to your personalization service instance; e.g. "0123456789abcdef0123456789ABCDEF"
    private static String ApiKey = "";

    // The endpoint specific to your personalization service instance; e.g. https://westeurope.api.cognitive.microsoft.com
    private static String ServiceEndpoint = "";

    public static void main(String[] args) {

        int iteration = 1;
        boolean runLoop;

        // Initialize Personalization client.
        PersonalizationClientImpl client = initializePersonalizationClient(ServiceEndpoint, ApiKey);

        // Get the actions list to choose from personalization with their features.
        List<RankableAction> actions = getActions();

        do {
            System.out.println("\nIteration: " + iteration++);

            // Get context information from the user.
            String timeOfDayFeature = getUsersTimeOfDay();
            String tasteFeature = getUsersTastePreference();

            // Create current context from user specified data.
            List<Object> contextFeatures = List.of(
                    new Object() {
                        String time = timeOfDayFeature;
                    },
                    new Object() {
                        String taste = tasteFeature;
                    }
            );

            // Exclude an action for personalization ranking. This action will be held at its current position.
            List<String> excludeActions = List.of("juice");

            // Generate an ID to associate with the request.
            String eventId = java.util.UUID.randomUUID().toString();

            // Rank the actions
            RankRequest personalizationRequest = new RankRequest()
                    .withActions(actions)
                    .withContextFeatures(contextFeatures)
                    .withExcludedActions(excludeActions)
                    .withEventId(eventId);
            RankResponse response = client.rank(personalizationRequest);

            System.out.println("\nPersonalization service thinks you would like to have: " + response.rewardActionId() + ". Is this correct? (y/n)");

            double reward = 0.0;
            String answer = getKey();

            if ("Y".equalsIgnoreCase(answer)) {
                reward = 1;
                System.out.println("\nGreat! Enjoy your food.");
            } else if ("N".equalsIgnoreCase(answer)) {
                reward = 0;
                System.out.println("\nYou didn't like the recommended food choice.");
            } else {
                System.out.println("\nEntered choice is invalid. Service assumes that you didn't like the recommended food choice.");
            }

            System.out.println("\nPersonalization service ranked the actions with the probabilities as below:");

            for (RankedAction rankedResponse : response.ranking()) {
                System.out.println(rankedResponse.id() + " " + rankedResponse.probability());
            }

            // Send the reward for the action based on user response.
            client.reward(response.eventId(), new RewardRequest().withValue(reward));

            System.out.println("\nPress q to break, any other key to continue:");
            runLoop = !("Q".equalsIgnoreCase(getKey()));

        } while (runLoop);
    }

    /**
     * Initializes the personalization client.
     * @param url : The endpoint specific to your personalization service instance.
     * @param apiKey : key specific to your personalization service instance.
     * @return Personalization client instance.
     */
    private static PersonalizationClientImpl initializePersonalizationClient(String url, String apiKey) {
        RestClient restClient = new RestClient.Builder()
                .withBaseUrl(url)
                .withInterceptor(chain -> {
                    Request original = chain.request();
                    Request request = original.newBuilder()
                            .header("Ocp-Apim-Subscription-Key", apiKey)
                            .method(original.method(), original.body())
                            .build();
                    return chain.proceed(request);
                })
                .withResponseBuilderFactory(new Factory())
                .withSerializerAdapter(new JacksonAdapter())
                .build();
        return new PersonalizationClientImpl(restClient);
    }

    /**
     * Get users time of the day context.
     * @return Time of day feature selected by the user.
     */
    private static String getUsersTimeOfDay() {
        String[] timeOfDayFeatures = new String[]{"morning", "afternoon", "evening", "night"};


        System.out.println("\nWhat time of day is it (enter number)? 1. morning 2. afternoon 3. evening 4. night");
        int timeIndex = tryParse(getKey());
        if (timeIndex < 1 || timeIndex > timeOfDayFeatures.length) {
            System.out.println("\nEntered value is invalid. Setting feature value to " + timeOfDayFeatures[0] + ".");
            timeIndex = 1;
        }

        return timeOfDayFeatures[timeIndex - 1];
    }

    /**
     * Gets user food preference.
     * @return Food taste feature selected by the user.
     */
    private static String getUsersTastePreference() {
        String[] tasteFeatures = new String[]{"salty", "sweet"};

        System.out.println("\nWhat type of food would you prefer (enter number)? 1. salty 2. sweet");
        int tasteIndex = tryParse(getKey());
        if (tasteIndex < 1 || tasteIndex > tasteFeatures.length) {
            System.out.println("\nEntered value is invalid. Setting feature value to " + tasteFeatures[0] + ".");
            tasteIndex = 1;
        }

        return tasteFeatures[tasteIndex - 1];
    }

    /**
     * Creates personalization actions feature list.
     * @return List of actions for personalization.
     */
    private static List<RankableAction> getActions() {

        return List.of(
                new RankableAction()
                        .withId("pasta")
                        .withFeatures(List.of(
                                new Object() {
                                    String taste = "salty";
                                    String spiceLevel = "medium";
                                },
                                new Object() {
                                    int nutritionLevel = 5;
                                    String cuisine = "italian";
                                })),

                new RankableAction()
                        .withId("ice cream")
                        .withFeatures(List.of(
                                new Object() {
                                    String taste = "sweet";
                                    String spiceLevel = "none";
                                },
                                new Object() {
                                    int nutritionLevel = 2;
                                })),

                new RankableAction()
                        .withId("juice")
                        .withFeatures(List.of(
                                new Object() {
                                    String taste = "sweet";
                                    String spiceLevel = "none";
                                },
                                new Object() {
                                    int nutritionLevel = 5;
                                    boolean drink = true;
                                })),

                new RankableAction()
                        .withId("salad")
                        .withFeatures(List.of(
                                new Object() {
                                    String taste = "salty";
                                    String spiceLevel = "low";
                                },
                                new Object() {
                                    int nutritionLevel = 8;
                                }))
        );
    }

    private static String getKey() {
        return new Scanner(System.in).next().substring(0, 1);
    }

    private static int tryParse(String c) {
        int retVal;
        try {
            retVal = Integer.parseInt(c);
        } catch (NumberFormatException nfe) {
            retVal = 0;
        }
        return retVal;
    }
}
