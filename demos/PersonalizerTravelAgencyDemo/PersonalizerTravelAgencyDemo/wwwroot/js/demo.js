let context = {
    device: "mobile",
    packageAdditionals: null,
    costs: null,
    userAgent: null
};

const ACTION_VIEWS = {
    HTML: "HTML",
    JSON: "JSON"
};

let userAgent = {};

let actionDisplayState = {
    selectedView: ACTION_VIEWS.HTML,
    currentActionId: ""
};

document.addEventListener("DOMContentLoaded", function () {
    const goBtnEle = document.getElementById("go-btn");
    const brandLogoImg = document.getElementById("brand-logo");
    const mobileShowBackstageBtn = document.getElementById("mobile-show-backstage-btn");
    const mobileHideBackstageBtn = document.getElementById("mobile-hide-backstage-btn");
    const navbar = document.getElementById('navbar-container');
    const articleContainer = document.getElementById('article-container');
    const graphContainer = document.getElementById('graph-container');
    const backstage = document.getElementById('collapseBackstage');
    const backstageBtn = document.getElementById("backstage-btn");
    const showActionJsonBtn = document.getElementById("showActionsJson");
    const showActionHtmlBtn = document.getElementById('showActionsHtml');

    const costsOptions = ["allInclusive", "luxuryPackage"];
    const additionalOptions = ["boatTrip", "dinnerAndBreakfast"];

    showActionHtmlBtn.style.display = 'none';

    let currentSize;
    let gaugeInterval = -1;
    const SCREEN_SIZE_SMALL = 0;
    const SCREEN_SIZE_BIG = 1;
    const mobileSize = 991;

    context.costs = getRandomOption(costsOptions);
    context.packageAdditionals = getRandomOption(additionalOptions);

    showActionJsonBtn.addEventListener('click', function () {
        actionDisplayState.selectedView = ACTION_VIEWS.JSON;
        showActionHtmlBtn.style.display = 'flex';
        showActionJsonBtn.style.display = 'none';
        showAction(actionDisplayState.currentActionId, ACTION_VIEWS.HTML);
    });

    showActionHtmlBtn.addEventListener("click", function () {
        actionDisplayState.selectedView = ACTION_VIEWS.HTML;
        showActionJsonBtn.style.display = 'flex';
        showActionHtmlBtn.style.display = 'none';
        showAction(actionDisplayState.currentActionId, ACTION_VIEWS.JSON);
    });

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

    goBtnEle.addEventListener("click", function () {
        updateRecommendation();
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
        const gauge = articleViewer.contentWindow.document.getElementById("gauge");
        const boundSetIframeContentSize = setIframeContentSize.bind(null, mainContainer);
        const modalButton = articleViewer.contentWindow.document.getElementById('endModal-close-button');
        const modalIcon = articleViewer.contentWindow.document.getElementById('endModal-close-icon');

        getRecommendation().then(result => {
            personalizerCallResult = result;
        });

        let reward = RewardInitValue;

        function sendRewardHandler(reward) {
            clearInterval(gaugeInterval);
            sendReward(personalizerCallResult.eventId, reward);
            var modalRewardText = articleViewer.contentWindow.document.getElementById("modal-reward");
            modalRewardText.textContent = Math.round(reward * 10) / 10;
        }

        clearInterval(gaugeInterval);
        gaugeInterval = -1;

        boundSetIframeContentSize(backstage.classList.contains('show'));

        backstageBtn.addEventListener('click', function () {
            boundSetIframeContentSize(!backstage.classList.contains('show'));
        });

        if (articleViewer.contentWindow.location.href.indexOf("onfirmation") > -1) {

            articleDoc.getElementById("btn-confirm").addEventListener("click", function () { sendRewardHandler(reward); });
            articleDoc.getElementById("link-save-later").addEventListener("click", function () {
                sendRewardHandler(SaveForLaterReward);
                updateRewardValue(SaveForLaterReward, articleDoc);
            });

            modalButton.addEventListener('click', goToHomeSite);

            modalIcon.addEventListener('click', goToHomeSite);

            updateShowGraphbtn(true);

            updateRewardValue(reward, articleDoc);

            gauge.addEventListener("transitionend", function gaugeTransitionEndHandler(event) {
                gauge.removeEventListener("transitionend", gaugeTransitionEndHandler);
                gaugeInterval = setInterval(function () {
                    reward -= RewardDecreaseAmount;
                    if (reward <= RewardDecreaseLimit) {
                        clearInterval(gaugeInterval);
                        gaugeInterval = -1;
                        updateRewardValue(RewardDecreaseLimit, articleDoc);
                    } else {
                        updateRewardValue(reward, articleDoc);
                    }

                }, RewardDecreaseInterval * 1000);
            }, false);

            var innerDoc = articleViewer.contentWindow.document;
            var iframeBackBtn = innerDoc.getElementById('iframe-backBtn');
            const gaugeContainerEle = innerDoc.getElementById('gauge-container');

            if (iframeBackBtn !== undefined) {
                iframeBackBtn.style.display = "block";
                iframeBackBtn.addEventListener("click", function () {
                    gaugeContainerEle.style.display = 'none';
                    articleViewer.contentWindow.history.back();
                });
            }

            brandLogoImg.addEventListener("click", function () {
                if (iframeBackBtn !== undefined) {
                    gaugeContainerEle.style.display = 'none';
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
    const turnValue = value / 2;
    const rewardEle = articleDoc.getElementById('gauge');
    rewardEle.setAttribute('style', `transform:rotate(${turnValue}turn)`);
    const comment = articleDoc.getElementById('gauge-comment');
    comment.innerText = `${value.toFixed(1)}`;
}

function setupActionControls() {
    getActions(false).then(updateActionsTab);
}

function setupContextControls() {
    const deviceSelectEle = document.getElementById('device');

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

    const costSelectEle = document.getElementById('costs');
    costSelectEle.addEventListener('change', (event) => {
        updateContext(null, event.target.value, null, false, null);
    });

    const packageSelectEle = document.getElementById('packageAdditionals');
    packageSelectEle.addEventListener('change', (event) => {
        updateContext(null, null, event.target.value, false, null);
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
            packageAdditionals: context.packageAdditionals
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

    actionDisplayState.currentActionId = actions[0].id;

    for (var i = 0; i < actions.length; i++) {
        let actionTabContent = createActionTab(actions[i], i === 0);
        actionsTabHeadersString += actionTabContent.tabHeader;
        actionsTabContentString += actionTabContent.tabContent;
    }

    actionsHeaderTab.innerHTML = actionsTabHeadersString;
    actionsTabContent.innerHTML = actionsTabContentString;
}

function createActionTab(actionObj, active) {

    const action = actionObj.features.reduce((previous, current) => Object.assign(previous, current));

    return {
        tabHeader: `<a class="nav-link d-flex align-items-center${active ? " active" : ""}" id="${actionObj.id}-article-tab" data-toggle="pill" onclick="showAction(${actionObj.id})" href="#${actionObj.id}-article" role="tab" aria-controls="${actionObj.id}-article" aria-selected="${active ? "true" : "false"}">
                        <img class="rounded img-fluid" alt="Preview thumbnail for ${actionObj.id}" src="img/actions-thumbnails/${actionObj.id}.png"></img>
                    </a>`,
        tabContent: `<div class="tab-pane fade ${active && actionDisplayState.selectedView === ACTION_VIEWS.JSON ? "show active" : ""}" role="tabpanel" id="${actionObj.id}-article-${ACTION_VIEWS.JSON}" role="tabpanel" aria-labelledby="${actionObj.id}-article-tab">
                        <pre class="pre-scrollable border m-0 actions-height"><code>${JSON.stringify(actionObj, null, 2)}</code></pre>
                    </div>
                    <div class="tab-pane fade ${active && actionDisplayState.selectedView === ACTION_VIEWS.HTML ? "show active" : ""}" role="tabpanel" id="${actionObj.id}-article-${ACTION_VIEWS.HTML}" role="tabpanel" aria-labelledby="${actionObj.id}-article-tab">
                       <div class="m-1 actions-grid">
                          <div class="gr-1 gc-1">Layout</div>
                          <div class="gr-2 gc-1"><img class="${action.layout.toLowerCase().indexOf('layouta') > -1 ? 'action-border border-primary rounded' : 'action-border border-white rounded'}" id="layout-a" src="/img/layout-a.png" alt="Layout A" /></div>
                          <div class="gr-3 gc-1"><img class="${action.layout.toLowerCase().indexOf('layoutb') > -1 ? 'action-border border-primary rounded' : 'action-border border-white rounded'}" id="layout-b" src="/img/layout-b.png" alt="Layout B" /></div>
                          <div class="gr-4 gc-1"><img class="${action.layout.toLowerCase().indexOf('layoutc') > -1 ? 'action-border border-primary rounded' : 'action-border border-white rounded'}" id="layout-c" src="/img/layout-c.png" alt="Layout C" /></div>
                          
                          <div class="gr-1 gc-2">Image</div>
                          <div class="gr-2 gc-2"><img class="${action.image.fileName.toLowerCase().indexOf('caribbean') > -1 ? 'action-border border-primary rounded' : 'action-border border-white rounded'}" id="beach" src="/img/caribbean-thumbnail.jpg" alt="Beach" /></div>
                          <div class="gr-3 gc-2"><img class="${action.image.fileName.toLowerCase().indexOf('elephant') > -1 ? 'action-border border-primary rounded' : 'action-border border-white rounded'}" id="pool" src="/img/elephant-thumbnail.jpg" alt="Desert" /></div>
                          
                          <div class="gr-1 gc-3">Tone & Font </div>
                          <div class="gr-2 gc-3"><img class="${action.toneFont.toLowerCase().indexOf('casual') > -1 ? 'action-border border-primary rounded' : 'action-border border-white rounded'}" id="casual" src="/img/casual.jpg" alt="Casual" /></div>
                          <div class="gr-3 gc-3"><img class="${action.toneFont.toLowerCase().indexOf('formal') > -1 ? 'action-border border-primary rounded' : 'action-border border-white rounded'}" id="formal" src="/img/formal.jpg" alt="Formal" /></div>
                          
                          <div class="gr-1 gc-4">Buy Button</div>
                          <div class="gr-2 gc-4"><img class="${action.buttonColor.indexOf(BlueButtonColor) > -1 ? 'action-border border-primary rounded' : 'action-border border-white rounded'}" id="blue" src="/img/buybutton-blue.jpg" alt="Blue" /></div>
                          <div class="gr-3 gc-4"><img class="${action.buttonColor.indexOf(OrangeButtonColor) > -1 ? 'action-border border-primary rounded' : 'action-border border-white rounded'}" id="orange" src="/img/buybutton-orange.jpg" alt="Orange" /></div>
                       </div>
                    </div>`
    };
}

function showAction(id, activeActionView) {
    id = id ? id : actionDisplayState.currentActionId;
    activeActionView = activeActionView ? activeActionView : actionDisplayState.selectedView;

    const activeAction = document.getElementById(`${actionDisplayState.currentActionId}-article-${activeActionView}`);
    activeAction.classList.remove("show", "active");

    actionDisplayState.currentActionId = id;

    const actionToActivate = document.getElementById(`${actionDisplayState.currentActionId}-article-${actionDisplayState.selectedView}`);
    actionToActivate.classList.add("show", "active");
}

function updateArticle(result) {
    const articleViewer = document.getElementById("article-viewer");
    articleViewer.src = `/home/Confirmation?actionId=${result.rewardActionId}`;
}

function getActions() {
    return fetch(`/api/Metadata/Actions`).then(r => r.json());
}

function getRecommendation() {
    const requestContext = {
        device: context.device,
        costs: context.costs,
        packageAdditionals: context.packageAdditionals,
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

function updateRecommendation() {
    getRecommendation().then(result => {
        personalizerCallResult = result;
        updateBasedOnRecommendation(result);
    });
}

function goToHomeSite() {
    const articleViewer = document.getElementById("article-viewer");
    articleViewer.src = '/home/HomeSite';
}