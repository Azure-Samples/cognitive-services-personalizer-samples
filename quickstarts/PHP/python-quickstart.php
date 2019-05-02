<?php
    require_once(__DIR__ . '\php\SwaggerClient-php\vendor\autoload.php');

    # This code is not verified as swagger-code generated client does not allow arbitary object
    # as Rank feature. Every object user wants to use has to implement several interfaces like 
    # RankedAction. This will give terrible user experience.
    # Hold this example till we have an official client release.

    $timeOfDayFeature = GetUsersTimeOfDay();
    $tasteFeature = GetUsersTastePreference();
    $action = GetActions();


    // Create current context from user specified data.

    $currentContext = [(object)['time' => $timeOfDayFeature],
                       (object)['taste'=> $tasteFeature]];

    $data = '{ "name": "Aragorn", "race": "Human" }'; $character = json_decode($data);

    $excludeActions = ['juice'];

    // Generate an ID to associate with the request.

    $eventId = uniqid();

    $rankRequest = (new Swagger\Client\Model\RankRequest())
                        ->setEventId($eventId)
                        ->setActions($action)
                        ->setContextFeatures([])
                        ->setExcludedActions($excludeActions);

    $a = $rankRequest->__toString();

    // Rank the actions
    $rankApiInstance = new Swagger\Client\Api\RankingApi(
        new GuzzleHttp\Client(),
        GetConfig()
    );
    
    try {
        $rankApiInstance->rankAsync($rankRequest)->then(function($response){
            echo 'received rank response', $response;
            // Rank the actions
             $rewardApiInstance = new Swagger\Client\Api\EventsApi(
                new GuzzleHttp\Client(),
                GetConfig()
            );

            $rewardApiInstance->rewardAsync($eventId)->then(function($response){
                echo 'received reward response', $response;
            });
        });

    } catch (Exception $e) {
        echo 'Exception when calling EventsApi->activate: ', $e->getMessage(), PHP_EOL;
    }

    function GetConfig()
    {
        ## Put your host and key
        $host = '{host}';
        $key = '{key}';
        
        // Configure API key authorization: apim_key
        return Swagger\Client\Configuration::getDefaultConfiguration()
            ->setApiKey('Ocp-Apim-Subscription-Key', $key)
            ->setHost($host);
    }

    function GetActions()
    {
        $action1 = (new Swagger\Client\Model\RankableAction)
            ->setId('pasta')
            ->setFeatures([(object)['taste' => 'salty','spicyLevel' => 'medium'],
                        (object)['nutritionLevel' => '5','cuisine' => 'italian']]);
        return [$action1];
    }

    function GetUsersTimeOfDay()
    {
        $timeOfDayFeatures = ['morning', 'afternoon', 'evening', 'night'];
        return $timeOfDayFeatures[rand(0,3)];
    }

    function GetUsersTastePreference()
    {
        $tasteFeatures = ["salty", "sweet"];
        return $tasteFeatures[rand(0,1)];
    }
?>
