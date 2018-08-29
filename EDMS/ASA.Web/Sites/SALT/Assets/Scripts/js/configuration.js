define({
    maps: {
        endPoint: '/Assets/Configs/TuitionWaiverStates.json',
        endPointlocation: '/Assets/Configs/locations.json',
        key: 'AIzaSyC6y_Sj3UiXcFP7N8hLwNGtUkv6UZSXCLM',
        styles: [
            {url: '/Assets/images/markercluster/clust1.png', height: 53, width: 52, anchor: [0, 0], textColor: 'white', textSize: 14},
            {url: '/Assets/images/markercluster/clust2.png', height: 56, width: 55, anchor: [0, 0], textColor: 'black', textSize: 14},
            {url: '/Assets/images/markercluster/clust3.png', height: 66, width: 65, anchor: [0, 0], textColor: 'white', textSize: 14}
        ]
    },
    internshipsIframe: {
        desktop: 'https://nonprod-idp.saltmoney.org/SSO/SSOLogin?PartnerName=SaltIDP/Internships/PSP_OAuthDevConnection_To_Internships',
        mobile: 'https://nonprod-idp.saltmoney.org/SSO/SSOLogin?PartnerName=SaltIDP/Internships/PSP_OAuthDevConnection_To_Internships&optionalParam=mobileInternships'
    },
    apiEndpointBases : {
        AlertServiceEndpoint: '/api/AlertService',
        MemberService: '/api/ASAMemberService',
        LoanServiceEndpoint: '/api/LoanService',
        ReminderService: '/api/ReminderService',
        SearchServiceEndpoint: '/api/SearchService',
        SelfReportedServiceEndpoint: '/api/SelfReportedService',
        SurveyServiceEndpoint: '/api/SurveyService',
        AuthHome: '/api/SearchService/GetMedia/salt-authoring/AuthHome',
        CustomLanding: '/api/SearchService/GetMedia/salt-authoring/CustomLanding',
        SearchPage: '/api/SearchService/GetSearch/salt-authoring/SearchResults/_/N-14',
        UnAuthHome: '/api/SearchService/GetMedia/salt-authoring/CombinedUnauthHome',
        GenericEndeca: '/api/SearchService/GetMedia/salt-authoring/',
        CCPPage: '/api/SearchService/GetMedia/salt-authoring/CCP',
        IsAuthenticated: 'true',
        FormsAuthTimeoutValue: 30,
        LoginRedirectPage: '/Home/RedirectLogOn',
        MasterMoney: 'DashboardMasterMoney?Ntk=Tags&Ntx=mode+matchany&Ntt=%22budgeting%22+%22banking%22+%22credit%22+%22taxes%22+%22insurance%22',
        FindAJob: 'DashboardFindAJob?Ntk=Tags&Ntx=mode+matchany&Ntt=%22job%20search%22+%22job%20applications%22+%22job%20interviews%22+%22internships%22+%22salaries%22+%22career%20skills%22',
        RepayStudentDebt: 'DashboardRepayStudentDebt?Ntk=Tags&Ntx=mode+matchany&Ntt=%22payment%20plans%22+%22postponements%22+%22cancellations%22+%22late%20payments%22',
        PayForSchool: 'DashboardPayForSchool?Ntk=Tags&Ntx=mode+matchany&Ntt=%22scholarships%22+%22federal%20student%20aid%22+%22private%20loans%22+%22transferring%22+%22school%20choice%22'
    },
    goalTags: {
        MasterMoney: 'budgeting,banking,credit,taxes,insurance',
        RepayStudentDebt: 'payment plans,late payments,postponements,cancellations',
        FindAJob: 'job search,job applications,job interviews,internships,salaries,career skills',
        PayForSchool: 'scholarships,federal student aid,private loans,transferring,school choice'
    },
    mm101: {
        url: 'https://oldmoney.remote-learner.net/auth/saml/index.php'
    },
    chatbotAgents: {
        SLR: {
            projectId: 'cps-proto-orig'
        }
    }
});
