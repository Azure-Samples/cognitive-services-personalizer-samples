import * as ko from 'knockout';
import 'isomorphic-fetch';

interface RankRequest {
    actions: object[],
    contextFeatures: object[],
    excludedActions: string[],
    eventId: string,
    deferActivation: boolean

}

interface RankResponse {
    ranking: object[],
    eventId: string,
    rewardActionId: string
}

interface RewardRequest {
    value: number
}

class HomePageViewModel {
    public rankRequest = ko.observable<RankRequest>();
    public rankResponse = ko.observable<RankResponse>();
    public rewardValue = ko.observable<number>(1);
    //public rewardRequest = ko.computed({
    //    owner: this,
    //    read: () => {
    //        return { value: this.rewardValue };
    //    }
    //});
    public rewardRequest = ko.observable<RewardRequest>();
    public rewardResponse = ko.observable<string>();

    public rewardValueWarning = ko.computed(() => {
        if (this.rewardValue() > 1) {
            return "Warning: a reward value of greater than 1 is not recommended"
        }
        else if (this.rewardValue() < 0) {
            return "Warning: a reward value of less than 0 is not recommended"
        }
        else {
            return "";
        }
    })

    public userAgent() {
        return this.rankRequest() ? this.prettify(this.rankRequest().contextFeatures[2]) : "Loading...";
    };

    public eventId() {
        return this.rankRequest() ? this.rankRequest().eventId : "n/a";
    };

    public rewardActionId() {
        return this.rankResponse() ? this.rankResponse().rewardActionId : "n/a";
    }

    public prettify(o: object): string {
        return o ? JSON.stringify(o, null, 4) : '';
    }

    constructor() {
        this.GenerateRankRequest();
        this.GenerateRewardRequest();

        this.rewardValue.subscribe(() => {
            this.GenerateRewardRequest()
        })
    }

    public GenerateRankRequest() {
        fetch('api/Personalizer/GenerateRank')
            .then(response => response.json() as Promise<RankRequest>)
            .then(data => this.rankRequest(data))
            .then(() => {
                this.rankResponse(null);
                this.rewardResponse('');
            })
    }

    public SendRankRequest() {
        fetch('api/Personalizer/PostRank',
            {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(this.rankRequest())
            })
            .then(response => response.json() as Promise<RankResponse>)
            .then(data => {
                this.rankResponse(data);
            })
    }

    public GenerateRewardRequest() {
        fetch('api/Personalizer/GenerateReward',
            {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(this.rewardValue())
            })
            .then(response => response.json() as Promise<RewardRequest>)
            .then(data => {
                this.rewardRequest(data);
            })
    }

    public SendRewardRequest() {
        fetch('api/Personalizer/PostReward/' + this.eventId(),
            {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(this.rewardRequest())
            })
            .then(response => response.text() as Promise<string>)
            .then(data => {
                this.rewardResponse(data);
            });
    }
}

export default { viewModel: HomePageViewModel, template: require('./home-page.html') };
