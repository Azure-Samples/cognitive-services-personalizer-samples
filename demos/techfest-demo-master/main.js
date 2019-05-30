const states =
{
  greeting: {
    action: async (context) => {
      let response = "Would you like to see what's available or get a coffee recommendation?";
      return { response, context };
    },
    transitions: [
      {
        text: "See what's available",
        next_state: "menu"
      },
      {
        text: "Give me a recommendation",
        next_state: "recommend"
      }
    ]
  },
  menu: {
    action: async (context) => {
      let response = getCoffeeList(coffees).join(", ");
      return { response, context };
    },
    transitions: [
      {
        text: "What should I get?",
        next_state: "recommend"
      }
    ]
  },
  recommend: {
    action: async (context) => {
      let features = context.userFeatureProvider.generateContextFeatures(context.featureData);
      let temp = await rank(features);

      // Demo code, should be more generic based on coffees object.
      let an_or_a;
      switch(temp.response.rewardActionId) {
        case "Espresso":
        case "Iced Coffee":
          an_or_a = "an";
          break;
        case "Latte":
        case "Mocha":
        case "Cold Brew":
          an_or_a = "a";
          break;
        default:
          an_or_a = "a";
      }

      let response = `How about ${an_or_a} ${temp.response.rewardActionId}?`;
      let debug = {};
      debug.response = temp.response;
      debug.context = temp.context;

      // Set the state object for the last event id to be used for rewards.
      context.lastEventId = temp.response.eventId;

      return { response, context, debug_type: "rank", debug };
    },
    transitions: [
      {
        text: "I like it",
        next_state: "like"
      },
      {
        text: "I don't like it",
        next_state: "dislike"
      },
      {
        text: "Give me another recommendation",
        next_state: "recommend"
      },
      {
        text: "Show me the menu",
        next_state: "menu"
      }
    ]
  },
  like: {
    action: async (context) => {
      let temp = await reward(context.lastEventId, 1.0);
      let debug = {};
      debug.response = temp.response;
      debug.context = temp.context;
      let response = "That's great! I'll keep learning your preferences over time.";
      return { response, context, debug_type: "reward", debug };
    },
    transitions: [
      {
        text: "Get another recommendation",
        next_state: "recommend"
      },
      {
        text: "Show me me the menu",
        next_state: "menu"
      }
    ]
  },
  dislike: {
    action: async (context) => {
      let temp = await reward(context.lastEventId, -1.0);
      let debug = {};
      debug.response = temp.response;
      debug.context = temp.context;
      let response = "Okay I'll remember that, would you like another recomendation?";
      return { response, context, debug_type: "reward", debug };
    },
    transitions: [
      {
        text: "Sure",
        next_state: "recommend"
      },
      {
        text: "Actually, show me the menu",
        next_state: "menu"
      }
    ]
  }
}

function getCoffeeList(coffeeData) {
  return coffeeData.map((x) => x.id);
}

class UserFeatureProvider {
  constructor() {
    this.providers = [];
  }

  addProvider(provider) {
    this.providers.push(provider);
  }

  generateContextFeatures(data) {
    let returnObject = [];
    for (let prov of this.providers) {
      returnObject.push(prov(data));
    }
    return returnObject;
  }
};

async function rank(contextFeatures) {
  let context = {};
  context.contextFeatures = contextFeatures;
  context.actions = coffees;
  context.excludeActions = null;
  context.activated = true;

  let response = await fetch("https://westus2.api.cognitive.microsoft.com/personalizer/v1.0/rank", {
    headers: {
      "Content-Type": "application/json; charset=utf-8",
      "Ocp-Apim-Subscription-Key": COG_SVC_KEY
    },
    method: "POST",
    body: JSON.stringify(context)
  });

  let result = {};
  result.response = await response.json();
  result.context = context;
  return result;
}

async function weather(lat, long) {
  let response = await fetch(`https://api.openweathermap.org/data/2.5/weather?lat=${lat}&lon=${long}&appid=${OPEN_WEATHER_MAP_KEY}`);
  return await response.json();
}

async function reward(eventId, value) {
  let context = {};
  context.value = value;

  let response = await fetch(`https://westus2.api.cognitive.microsoft.com/personalizer/v1.0/events/${eventId}/reward`, {
    headers: {
      "Content-Type": "application/json; charset=utf-8",
      "Ocp-Apim-Subscription-Key": COG_SVC_KEY
    },
    method: "POST",
    body: JSON.stringify(context)
  });

  let result = {};
  result.response = response;
  result.context = context;
  return result;
}

function add_button(parent, text, callback) {
  let element = document.createElement("button");
  element.classList.add("btn", "choice-btn");
  element.innerHTML = text;
  element.onclick = callback(element);
  parent.appendChild(element);
  return element;
}

function append_user_message(parent, text) {
  let outer = document.createElement("div");
  outer.className = "col-lg-12";
  let inner = document.createElement("div");
  inner.classList.add("speech-bubble-user", "speech-bubble");
  outer.appendChild(inner);

  let textElem = document.createElement("div");
  textElem.className = "speech-bubble-text";
  textElem.innerHTML = text;
  inner.appendChild(textElem);

  parent.appendChild(outer);
}

function injectTagForId(container, id, tag) {
  let idx = container.innerHTML.indexOf(id);
  let start = container.innerHTML.substring(0, idx - 1);
  let end =  container.innerHTML.substring(idx + 1 + id.length);
  container.innerHTML = start + '<span class="'+tag+'">"' + id + '"</span>' + end;
}

function append_bot_message(parent, text, transitions, states, context) {
  let outer = document.createElement("div");
  outer.className = "col-lg-12";
  let inner = document.createElement("div");
  inner.classList.add("speech-bubble-bot", "speech-bubble");
  outer.appendChild(inner);

  let textElem = document.createElement("div");
  textElem.className = "speech-bubble-text";
  textElem.innerHTML = text;
  inner.appendChild(textElem);

  parent.appendChild(outer);
  return inner;
}

function getCurrentLocation(options) {
  return new Promise((resolve, reject) => {
    navigator.geolocation.getCurrentPosition(resolve, ({code, message}) =>
      reject(Object.assign(new Error(message), {name: "PositionError", code})),
      options);
    });
};

function repeatEvery(interval, func) {
  // Check current time and calculate the delay until next interval
  var now = new Date(),
      delay = interval - now % interval;

  function start() {
      // Execute function now...
      func();
      // ... and every interval
      setInterval(func, interval);
  }

  // Delay execution until it's an even interval
  setTimeout(start, delay);
}

const ONE_MINUTE = 60 * 1000;

function formatAMPM(date) {
  var hours = date.getHours();
  var minutes = date.getMinutes();
  var ampm = hours >= 12 ? 'pm' : 'am';
  hours = hours % 12;
  hours = hours ? hours : 12; // the hour '0' should be '12'
  minutes = minutes < 10 ? '0'+minutes : minutes;
  var strTime = hours + ':' + minutes + ' ' + ampm;
  return strTime;
}

// Enable tooltips
$(function () {
  $('[data-toggle="tooltip"]').tooltip()
})

function setCookie(name,value,days) {
  var expires = "";
  if (days) {
      var date = new Date();
      date.setTime(date.getTime() + (days*24*60*60*1000));
      expires = "; expires=" + date.toUTCString();
  }
  document.cookie = name + "=" + (value || "")  + expires + "; path=/";
}
function getCookie(name) {
  var nameEQ = name + "=";
  var ca = document.cookie.split(';');
  for(var i=0;i < ca.length;i++) {
      var c = ca[i];
      while (c.charAt(0)==' ') c = c.substring(1,c.length);
      if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length,c.length);
  }
  return null;
}
function eraseCookie(name) {
  document.cookie = name+'=; Max-Age=-99999999;';
}

function resetSessions()
{
  eraseCookie("sessions");
}

window.onload = async () => {
  const sessions_container = document.getElementById("sessions");
  if(getCookie("sessions") === null)
  {
    setCookie("sessions", "0");
  }

  let num_sessions = parseInt(getCookie("sessions"));
  num_sessions += 1;
  setCookie("sessions", num_sessions.toString());
  sessions_container.innerHTML = num_sessions.toString();

  const info_time = document.getElementById("info-time");
  const info_location = document.getElementById("info-location");
  const info_weather = document.getElementById("info-weather");
  const info_icon_weather = document.getElementById("info-icon-weather");
  const chat_container = document.getElementById("chat-container");

  const response_container = document.getElementById("response");
  const context_container = document.getElementById("context");
  const reward_container = document.getElementById("reward");

  const exploit_container = document.getElementById("exploit-percentage");
  const exploit_name = document.getElementById("exploit-percentage-name");
  const exploit_percentage = document.getElementById("exploit-percentage-percentage");

  const explore_container = document.getElementById("explore-percentage");
  const explore_name = document.getElementById("explore-percentage-name");
  const explore_percentage = document.getElementById("explore-percentage-percentage");

  info_time.innerHTML = formatAMPM(new Date);
  repeatEvery(ONE_MINUTE, () => {
    info_time.innerHTML = formatAMPM(new Date);
  });

  let start_state = states.greeting;

  let context_transitions = {};
  context_transitions.lastEventId = "";
  context_transitions.featureData = {};

  let location = undefined;
  try {
    location = await getCurrentLocation();
  }
  catch(ex) {
    console.error("Location not available, location features turned off.");
    console.error(ex)
  }

  if(location !== undefined)
  {
    context_transitions.featureData.location = location;
    context_transitions.featureData.weather = await weather(location.coords.latitude, location.coords.longitude);
    info_location.innerHTML = context_transitions.featureData.weather.name;
    let weather_description = context_transitions.featureData.weather.weather[0].main;
    let weather_description_id = context_transitions.featureData.weather.weather[0].id;
    info_weather.innerHTML = weather_description;

    // https://openweathermap.org/weather-conditions
    if(weather_description_id >= 200 && weather_description_id < 300)
    {
      info_icon_weather.src = "./icon_weather_stormy.svg"
    }
    else if(weather_description_id >= 800 && weather_description_id < 900)
    {
      switch(weather_description.toLowerCase())
      {
        case "few clouds":
        case "scattered clouds":
          info_icon_weather.src = "./icon_weather_partly-cloudy.svg";
        break;
        case "broken clouds":
        case "overcast  clouds":
          info_icon_weather.src = "./icon_weather_cloudy.svg"
          break;
      }
    }
    else if(weather_description_id >= 300 && weather_description_id < 600)
    {
      info_icon_weather.src = "./icon_weather_rainy.svg";
    }

  }

  let userFeatureProvider = new UserFeatureProvider();
  userFeatureProvider.addProvider(data => {
    return { "day": data.date.getDay() }
  });

  userFeatureProvider.addProvider(data => {
    let hour = data.date.getHours();
    let time;
    if(hour < 7 || hour > 19)
    {
      time = "night";
    } else if (hour >= 7 || hour < 12) {
      time = "morning";
    } else {
      time = "afternoon";
    }

    return { "timeOfDay": time }
  });

  // Location is required for weather, city, season, temperature
  if(location !== undefined) {
    userFeatureProvider.addProvider(data => {
      return { "city": data.weather.name }
    });

    userFeatureProvider.addProvider(data => {
      return { "temperature": data.weather.main.temp }
    });

    userFeatureProvider.addProvider(data => {
      return { "weather": data.weather.weather[0].main }
    });

    userFeatureProvider.addProvider(data => {
      let month = data.date.getMonth();

      // If in southern hemisphere offset months by 6 to get correct season.
      if(data.location.coords.latitude < 0)
      {
        month = (month + 6) % 12;
      }

      let season = "";
      switch(month) {
        case 12:
        case 1:
        case 2:
            season = "winter";
        break;
        case 3:
        case 4:
        case 5:
            season = "spring";
        break;
        case 6:
        case 7:
        case 8:
            season = "summer";
        break;
        case 9:
        case 10:
        case 11:
            season = "fall";
        break;
      }
      return { "season": season };
    });

  }

  context_transitions.userFeatureProvider = userFeatureProvider;
  context_transitions.featureData.date = new Date();

  const go_to_state = async (current_state, state_context) => {

    let result = await current_state.action(state_context);
    let button_container = append_bot_message(chat_container, result.response,current_state.transitions);
    let buttons = [];
    for (let action of current_state.transitions) {
      buttons.push(add_button(button_container, action.text, (self) => {
        return () => {
          buttons.forEach(button => button.disabled = true);
          self.classList.add("selected");
          append_user_message(chat_container, action.text);
          go_to_state(states[action.next_state], result.context)
        }
      }));
    }

    $(chat_container).animate({
        scrollTop: chat_container.scrollHeight
    }, 200);

    if (result.hasOwnProperty("debug_type")) {
      if (result.debug_type == "rank") {
        context_container.innerHTML = "";
        response_container.innerHTML = "";
        reward_container.innerHTML = "";
        context_container.appendChild(document.createTextNode(JSON.stringify(result.debug.context, null, 2)));
        response_container.appendChild(document.createTextNode(JSON.stringify(result.debug.response, null, 2)));

          if(result.debug.response.ranking[0].probability > result.debug.response.ranking[1].probability)
        {
          injectTagForId(response_container, result.debug.response.ranking[0].id, "exploit-tag");
          exploit_container.style.display = "block";
          exploit_name.innerHTML = result.debug.response.ranking[0].id;
          exploit_percentage.innerHTML = result.debug.response.ranking[0].probability;
        }
        else
        {
          let exploreProb = result.debug.response.ranking[0].probability;
          let exploitItem = result.debug.response.ranking[0];
          for (let item of result.debug.response.ranking) {
            if(item.probability > exploreProb)
            {
              exploitItem = item;
            }
          }
          injectTagForId(response_container, exploitItem.id, "exploit-tag");
          injectTagForId(response_container, result.debug.response.ranking[0].id, "explore-tag");

          exploit_container.style.display = "block";
          exploit_name.innerHTML = exploitItem.id;
          exploit_percentage.innerHTML = exploitItem.probability;

          explore_container.style.display = "block";
          explore_name.innerHTML = result.debug.response.ranking[0].id;
          explore_percentage.innerHTML = result.debug.response.ranking[0].probability;
        }
      }
      else if (result.debug_type == "reward") {
        reward_container.appendChild(document.createTextNode(JSON.stringify(result.debug.context, null, 2)));
        if(result.debug.context.value > 0)
        {
          reward_container.innerHTML += '<p class="p-desc">A <span class="p-highlight">positive</span> reward was sent for this action. The model will learn over time that this is a good action for the context.</p>';
        }
        else {
          reward_container.innerHTML += '<p class="p-desc">A <span class="p-highlight">negative</span> reward was sent for this action. The model will learn over time that this is not a good action for the context.</p>';
        }
      }
    }
  }

  go_to_state(start_state, context_transitions);
};
