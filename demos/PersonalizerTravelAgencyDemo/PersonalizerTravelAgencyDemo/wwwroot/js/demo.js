let context = {
    device: "mobile",
    packageAdditionals: null,
    costs: null,
    userAgent: null
};

let userAgent = {};

document.addEventListener("DOMContentLoaded", function () {
    const timeleftContainer = document.getElementById("timeleft-container");
    const goBtnEle = document.getElementById("go-btn");
    const brandLogoImg = document.getElementById("brand-logo");
    const mobileShowBackstageBtn = document.getElementById("mobile-show-backstage-btn");
    const mobileHideBackstageBtn = document.getElementById("mobile-hide-backstage-btn");
    const navbar = document.getElementById('navbar-container');
    const articleContainer = document.getElementById('article-container');
    const graphContainer = document.getElementById('graph-container');
    const backstage = document.getElementById('collapseBackstage');
    const backstageBtn = document.getElementById("backstage-btn");

    const costsOptions = ["allInclusive", "luxury"];
    const additionalOptions = ["boatTrip", "dinnerBreakfast"];

    let currentSize;
    let gaugeInterval = -1;
    const SCREEN_SIZE_SMALL = 0;
    const SCREEN_SIZE_BIG = 1;
    const mobileSize = 991;
    let intervalId = -1;
    let reward = 0;

    context.costs = getRandomOption(costsOptions);
    context.packageAdditionals = getRandomOption(additionalOptions);

    backstageBtn.addEventListener("click", function () {
        backstageBtn.innerText = backstage.classList.contains('show') ? MainArticleShowBackstageLabel : MainArticleCloseBackstageLabel;
    });


    mobileShowBackstageBtn.addEventListener("click", function () {
        if (!backstage.classList.contains('show')) {
            hidePageContent();
        }
    });

    mobileHideBackstageBtn.addEventListener("click", function () {
        if (backstage.classList.contains('show')) {
            showPageContent();
        }
    });

    let personalizerCallResult;

    setupActionControls();
    setupContextControls();

    if (!startDemoWithBlankPage) {
        getRecommendation().then(result => {
            personalizerCallResult = result;
            updateBasedOnRecommendation(result);
        });
    }

    if (document.documentElement.clientWidth > mobileSize) {
        currentSize = SCREEN_SIZE_BIG;
    }
    else {
        currentSize = SCREEN_SIZE_SMALL;
    }

    window.onresize = function () {
        if (window.innerWidth > mobileSize) {
            if (currentSize === SCREEN_SIZE_SMALL) {
                currentSize = SCREEN_SIZE_BIG;
                setBigLayoutConfiguration();
            }
        } else {
            if (currentSize === SCREEN_SIZE_BIG) {
                currentSize = SCREEN_SIZE_SMALL;
                setSmallLayoutConfiguration();
            }
        }
    };

    //goBtnEle.addEventListener("click", function () {
    //    getRecommendation().then(result => {
    //        personalizerCallResult = result;
    //        updateBasedOnRecommendation(result);
    //    });
    //});

    goBtnEle.addEventListener("click", function () {
        const articleViewer = document.getElementById("article-viewer");
        articleViewer.src = `/home/confirmation`;
    });

    function setIframeContentSize(mainContainer, isBackStageOpen) {
        if (isBackStageOpen) {
            mainContainer.className = "col-12";
        }
        else {
            mainContainer.className = "col-xl-8 offset-xl-2 col-12";
        }
    }

    function setBigLayoutConfiguration() {
        if (backstage.classList.contains('show')) {
            showPageContent();
            backstageBtn.firstChild.data = MainArticleCloseBackstageLabel;
        } else {
            backstageBtn.firstChild.data = MainArticleShowBackstageLabel;
        }
    }

    function setSmallLayoutConfiguration() {
        if (backstage.classList.contains('show')) {
            hidePageContent();
        }
    }

    // Hides the page content except for the backstage
    function showPageContent() {
        navbar.style.display = 'flex';
        articleContainer.style.display = 'block';
        graphContainer.style.display = 'flex';
    }

    // Makes the page content visible except for the backstage which will remain unchanged
    function hidePageContent() {
        navbar.style.display = 'none';
        articleContainer.style.display = 'none';
        graphContainer.style.display = 'none';
    }

    const articleViewer = document.getElementById("article-viewer");
    articleViewer.addEventListener("load", function () {
        const articleDoc = articleViewer.contentDocument;
        const mainContainer = articleViewer.contentWindow.document.getElementById("main-container");
        const articleFooter = articleViewer.contentWindow.document.getElementById("article-footer");
        const boundSetIframeContentSize = setIframeContentSize.bind(null, mainContainer);

        boundSetIframeContentSize(backstage.classList.contains('show'));

        backstageBtn.addEventListener('click', function () {
            boundSetIframeContentSize(!backstage.classList.contains('show'));
        });

        if (articleViewer.contentWindow.location.href.indexOf("confirmation") > -1) {

            updateShowGraphbtn(true);
            if (intervalId >= 0) {
                clearInterval(intervalId);
                intervalId = -1;
            }

            let counter = 20;
            reward = 0.8;
            updateRewardValue(reward, articleDoc);
            clearRewardmessage();

            intervalId = setInterval(function () {
                counter--;
                timeleftContainer.innerHTML = `<p class="col-12 px-4 py-2 m-0" style="font-size: 1.4rem;">
                        <i class="fas fa-hourglass-half"></i> ${counter}s left to get reward
                    </p>`;
                if (counter <= 0) {
                    clearInterval(intervalId);
                    intervalId = -1;
                    sendReward(personalizerCallResult.eventId, reward).then(() => {
                        showRewardMessage(reward);
                    });
                    timeleftContainer.innerHTML = '';
                }
            }, 1000);

            gaugeInterval = setInterval(function () {
                const comment = articleDoc.getElementById('gauge-comment').innerText;
                let newValue = comment - RewardDecreaseAmount;
                if (newValue <= RewardDecreaseLimit) {
                    clearInterval(gaugeInterval);
                    gaugeInterval = -1;
                    updateRewardValue(RewardDecreaseLimit, articleDoc);
                } else {
                    updateRewardValue(newValue, articleDoc);
                }

            }, RewardDecreaseInterval*1000);
            

            var innerDoc = articleViewer.contentWindow.document;
            var iframeBackBtn = innerDoc.getElementById('iframe-backBtn');
            const gaugeContainerEle = innerDoc.getElementById('gauge-container');

            if (iframeBackBtn !== undefined) {
                iframeBackBtn.style.display = "block";
                iframeBackBtn.addEventListener("click", function () {
                    clearInterval(intervalId);
                    intervalId = -1;
                    timeleftContainer.innerHTML = '';

                    if (counter > 0) {
                        sendReward(personalizerCallResult.eventId, reward).then(() => {
                            showRewardMessage(reward);
                        });
                    }

                    gaugeContainerEle.style.display = 'none';
                    updateRewardValue(0, articleDoc);
                    clearRewardmessage();
                    counter = 0;
                    articleViewer.contentWindow.history.back();
                });
            }

            brandLogoImg.addEventListener("click", function () {
                if (iframeBackBtn !== undefined) {
                    clearInterval(intervalId);
                    intervalId = -1;
                    if (counter > 0) {
                        sendReward(personalizerCallResult.eventId, reward).then(() => {
                            showRewardMessage(reward);
                        });
                    }
                    timeleftContainer.innerHTML = '';
                    gaugeContainerEle.style.display = 'none';
                    updateRewardValue(0, articleDoc);
                    clearRewardmessage();
                    counter = 0;
                }

                articleViewer.contentWindow.history.back();
            });
        }
        else {
            updateShowGraphbtn(false);
        }
    });

});

function updateRewardValue(value, articleDoc) {
    const percentageValue = Math.round(value * 100);
    const turnValue = Math.round(percentageValue * 5 / 100);
    const rewardEle = articleDoc.getElementById('gauge');
    rewardEle.setAttribute('style', `transform:rotate(.${turnValue}turn)`);
    const comment = articleDoc.getElementById('gauge-comment');
    comment.innerText = `${value.toFixed(1)}`;
}

function showRewardMessage(reward) {
    const alertContainerEle = document.getElementById('alert-container');
    alertContainerEle.innerHTML = `<div class="alert alert-success col-12" role="alert">
        Reward of <strong>${reward}</strong> was sent to Personalizer
    </div>`;
}

function clearRewardmessage() {
    const alertContainerEle = document.getElementById('alert-container');
    cleanChilds(alertContainerEle);
}

function setupActionControls() {
    getActions(false).then(updateActionsTab);
}

function setupContextControls() {
    const deviceSelectEle = document.getElementById('device');
    deviceSelectEle.selectedIndex = ramdomizeSelectedOption(deviceSelectEle);
    deviceSelectEle.addEventListener('change', (event) => {
        updateContext(event.target.value, null, null, false, null);
    });

    const UseUserAgentEle = document.getElementById('use-useragent');
    UseUserAgentEle.addEventListener('change', (event) => {
        const checkbox = event.target;
        if (checkbox.checked) {
            updateContext(null, null, null, false, userAgent);
        } else {
            updateContext(null, null, null, true, null);
        }
    });

    getUserAgent().then(userAgentResponse => {
        userAgent = userAgentResponse;
        updateContext(deviceSelectEle.value, null, null, !UseUserAgentEle.checked, userAgent);
    });

    updateContext(deviceSelectEle.value, null, null, false, null);
}

function updateContext(device, currentCost, currentAdditionals, removeUserAgent, userAgent) {
    context.device = device || context.device;
    context.costs = currentCost || context.costs;
    context.packageAdditionals = currentAdditionals || context.packageAdditionals;
    context.userAgent = removeUserAgent ? null : userAgent || context.userAgent;

    let contextFeatures = [
        {
            device: context.device,
            costs: context.costs,
            additionals: context.packageAdditionals
        }
    ];


    if (context.userAgent) {
        contextFeatures.push({ userAgent: context.userAgent });
    }

    updateCodeElementWithJSON("context-code", { contextFeatures: contextFeatures });
}

function ramdomizeSelectedOption(select) {
    var items = select.getElementsByTagName('option');
    var index = Math.floor(Math.random() * items.length);

    return index;
}

function updateBasedOnRecommendation(result) {
    showResultContainer();
    hideResultAlert();
    updateArticle(result);
    updateResult(result);
    updatePersonalizerMethod(result);
}

function showResultContainer() {
    const resultContainerEle = document.getElementById("result-container");
    resultContainerEle.classList.remove("d-none");
}

function hideResultAlert() {
    const resultAlertElement = document.getElementById("result-alert");
    resultAlertElement.classList.add("d-none");
}

function updatePersonalizerMethod(recommendation) {
    const exploringBoxEle = document.getElementById("exploring-box");
    const exploitingBoxEle = document.getElementById("exploiting-box");

    if (isExploiting(recommendation)) {
        exploitingBoxEle.className = 'card border-left border-primary';
        exploringBoxEle.className = 'card';
    } else {
        exploringBoxEle.className = 'card border-primary';
        exploitingBoxEle.className = 'card';
    }
}

function isExploiting(recommendation) {
    const rewardActionId = recommendation.rewardActionId;
    const ranking = recommendation.ranking;

    let max = Math.max.apply(Math, recommendation.ranking.map((r) => { return r.probability; }));

    for (var i = 0; i < ranking.length; i++) {
        if (ranking[i].id === rewardActionId) {
            return ranking[i].probability === max;
        }
    }
}

function updateResult(result) {
    updateCodeElementWithJSON("result-code", { result: result }, result.rewardActionId);
}

function updateCodeElementWithJSON(eleId, jsonObj, resultId) {
    const codeEle = document.getElementById(eleId);
    let code = JSON.stringify(jsonObj, null, 2);

    if (resultId) {
        let aux = JSON.parse(code);
        aux = {
            result: {
                eventId: aux.result.eventId,
                rewardActionId: aux.result.rewardActionId,
                ranking: aux.result.ranking
            }
        };
        code = JSON.stringify(aux, null, 2);
        const regex = new RegExp(`(.*)("rewardActionId":\\s"${resultId}")(.*)`, 'gm');
        code = code.replace(regex, '$1<mark>$2</mark>$3');
    }

    codeEle.innerHTML = code;
}

function updateActionsTab(actions) {
    const actionsHeaderTab = document.getElementById("actions-tab");
    const actionsTabContent = document.getElementById("actions-tabContent");

    cleanChilds(actionsHeaderTab);
    cleanChilds(actionsTabContent);

    let actionsTabHeadersString = "";
    let actionsTabContentString = "";

    for (var i = 0; i < actions.length; i++) {
        let actionTabContent = createActionTab(actions[i], i === 0);
        actionsTabHeadersString += actionTabContent.tabHeader;
        actionsTabContentString += actionTabContent.tabContent;
    }

    actionsHeaderTab.innerHTML = actionsTabHeadersString;
    actionsTabContent.innerHTML = actionsTabContentString;
}

function createActionTab(actionObj, active) {
    let action = {};
    for (var attr in actionObj) {
        if (actionObj.hasOwnProperty(attr) && attr !== "title" && attr !== "imageName") action[attr] = actionObj[attr];
    }

    return {
        tabHeader: `<a class="nav-link d-flex align-items-center${active ? " active" : ""}" id="${actionObj.id}-article-tab" data-toggle="pill" href="#${actionObj.id}-article" role="tab" aria-controls="${actionObj}-article" aria-selected="${active ? "true" : "false"}"> ${actionObj.id}
                        <div class="mx-auto"></div>
                        <img class="rounded img-fluid" alt="Preview thumbnail for ${actionObj.title}" src="img/${actionObj.imageName}" style="max-width:4rem;"></img>
                    </a>`,
        tabContent: `<div class="tab-pane fade ${active ? "show active" : ""}" role="tabpanel" id="${actionObj.id}-article" role="tabpanel" aria-labelledby="${actionObj.id}-article-tab">
                        <p class="h6 p-1 pt-2 mb-0"><strong>Title:</strong> ${actionObj.title}</p>
                        <pre class="pre-scrollable border m-0 actionsjson"><code>${JSON.stringify(action, null, 2)}</code></pre>
                    </div>`
    };
}

function updateArticle(result) {
    let articleIds = result.ranking.map(function (ranking) {
        return ranking.id;
    }).join(",");
    const articleViewer = document.getElementById("article-viewer");
    articleViewer.src = `/home/homesite?articleIds=${articleIds}`;
}

function getActions() {
    return fetch(`/api/Metadata/Actions`).then(r => r.json());
}

function getRecommendation() {
    const requestContext = {
        device: context.device,
        costs: context.costs,
        additionals: context.packageAdditionals,
        useUserAgent: !!context.userAgent
    };

    return fetch("/api/Personalizer/Recommendation", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(requestContext)
    }).then(r => r.json());
}

function getUserAgent() {
    return fetch("/api/Metadata/UserAgent").then(r => r.json());
}

function sendReward(eventid, value) {
    return fetch("/api/Personalizer/Reward", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({
            eventid: eventid,
            value: value
        })
    });
}

function updateShowGraphbtn(shouldShow) {
    let previousClass = "visible";
    let actualClass = "invisible";

    if (shouldShow) {
        previousClass = "invisible";
        actualClass = "visible";
    }

    document.getElementById("learn-button").classList.replace(previousClass, actualClass);
    document.getElementById("mobile-learn-button").classList.replace(previousClass, actualClass);
}

function getRandomOption(options) {
    var randomNumber = Math.floor(Math.random() * options.length);

    return options[randomNumber];
}