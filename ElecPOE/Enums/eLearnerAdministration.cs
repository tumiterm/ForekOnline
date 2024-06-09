using System.ComponentModel.DataAnnotations;

namespace ElecPOE.Enums
{
    public enum eDocumentType
    {
        [Display(Name = "Knowledge Module" )]
        KM,
        [Display(Name = "Practical Module")]
        PM,
        [Display(Name = "Workplace Module")]
        WM,
        [Display(Name = "Student File")]
        File,
    }
    public enum eFileType
    {
        [Display(Name = "Training Schedule")]
        TrainingSchedule,
        Timetable,
        [Display(Name = "Assessment Plan")]
        AssessmentPlan,
        [Display(Name = "Monthly Report")]
        MonthlyReport,
        [Display(Name = "Study Material")]
        StudyMaterial,
        [Display(Name = "Attendance Registers")]
        Attendance,
        [Display(Name = "Acknowledgement Registers")]
        Acknowledgement,
        [Display(Name = "Document/Record Request Form")]
        Document
    }
    public enum ApplicationStatus
    {
        Submitted,
        Pending,
        Approved,
        Rejected,
        [Display(Name = "New Submitted")]
        NewSubmitted
    }

    public enum eLicenseFrequency
    {
        [Display(Name = "First Time")]
        First,
        [Display(Name = "Second Time")]
        Second,
        [Display(Name = "Third Time")]
        Third
    }

    public enum eClientType
    {
        Corporate = 12,
        Private = 13,
    }

    public enum HighestQualification
    {
        [Display(Name = "Grade 9")]
        Grade9,
        [Display(Name = "Grade 10")]
        Grade10,
        [Display(Name = "Grade 11")]
        Grade11,
        [Display(Name = "Grade 12")]
        Grade12,
        N1, N2,N3,N4,N5,N6,
        [Display(Name = "Higher Certificate")]
        HigherCertificate,
        Diploma,
        Degree,
        Honours,
        Masters,
        PhD
   
    }
    public enum eRelationship
    {
        Mother,Father,Uncle,Sister,Brother,Grandfather,Grandmother,
        Husband,Wife, Spouse, Aunt, Other
    }
    public enum eTitle
    {
        Adv,Capt,Dr,Ds,Miss,Ms,Mr,Prof,Rev
    }
    public enum eGender
    {
        Male,Female,Other
    }
    public enum eLearnerAdministration
    {
        [Display(Name= "Learner RSA ID")]
        LearnerID,
        [Display(Name = "Learner Passport")]
        LearnerPassport,
        [Display(Name = "Curriculum Vitae")]
        CV,
        [Display(Name = "Matric Certificate")]
        Matric,
        [Display(Name = "Learner Qualification")]
        Qualification,
        [Display(Name = "Study Permit")]
        StudyPermit,
        [Display(Name = "Parent/Guardian ID")]
        GuardianId,
        [Display(Name = "Proof of Residence")]
        Residence
    }
    public enum eTrainingAdministration
    {
        [Display(Name = "Enrollment Form")]
        EnrollmentForm,
        [Display(Name = "Induction Form")]
        InductionForm,
        [Display(Name = "Declaration of Authenticity")]
        Declaration,
        [Display(Name = "Code of Conduct")]
        Conduct,
        [Display(Name = "Learner Agreement")]
        Agreement,
        [Display(Name = "Training Schedule")]
        Schedule,
        Timetable,
        [Display(Name = "Learner Contract")]
        Contract

    }
    public enum eAssessmentAdministration
    {
        [Display(Name = "Formative Assessment")]
        Formative,
        [Display(Name = "Summative Assessment")]
        Summative,
        [Display(Name = "Practical Assessment")]
        Practical,
        IISA,
        Report,
        [Display(Name = "Formative 1")]
        Formative1,
        [Display(Name = "Formative 2")]
        Formative2,
        [Display(Name = "Formative 3")]
        Formative3,
    }
    public enum eSysRole
    {
        None,Admin,Facilitator,Student,SuperAdmin
    }
    public enum eChoice
    {
        Yes,No
    }
    public enum eCategory
    {
        [Display(Name = "Asylum Seeker")]
        AsylumSeeker,
        LSP,
        [Display(Name = "Permanent Residence Permit")]
        PermanentResidencePermit,
        [Display(Name = "Study Visa")]
        StudyVisa,
        [Display(Name = "Zimbabwe Exception Permit")]
        ZimbabweExceptionPermit

    }
    public enum eNType
    {
        N1,N2, N3, N4, N5, N6
    }
    public enum eOperationType
    {
        Course,
        Module,
        Company
    }
    public enum eStatus
    {
        [Display(Name = "Starting Soon")]
        StartingSoon,
        Started,
        [Display(Name = "Dropped Out")]
        DroppedOut,
        Transferred,
        Completed
    }
    public enum eProvince
    {
        Mpumalanga,
        Limpopo,
        [Display(Name ="Northern Cape")]
        NorthernCape,
        [Display(Name = "Western Cape")]
        WesternCape,
        [Display(Name = "Eastern Cape")]
        EasternCape,
        FreeState,
        [Display(Name = "Free State")]
        Gauteng,
        [Display(Name = "KwaZulu-Natal")]
        KwaZuluNatal,
        [Display(Name = "North West")]
        NorthWest
    }
    public enum eSpeciality
    {
        [Display(Name = "Technology Development")]
        TechnologyDevelopment,

        [Display(Name = "E-commerce")]
        ECommerce,

        [Display(Name = "Financial Services")]
        FinancialServices,

        Manufacturing,

        Healthcare,

        [Display(Name = "Energy and Utilities")]
        EnergyUtilities,

        [Display(Name = "Transportation and Logistics")]
        TransportationLogistics,

        [Display(Name = "Consulting and Professional Services")]
        ConsultingServices,

        [Display(Name = "Hospitality and Tourism")]
        HospitalityTourism,

        [Display(Name = "Media and Entertainment")]
        MediaEntertainment,

        [Display(Name = "Real Estate and Construction")]
        RealEstateConstruction,

        [Display(Name = "Retail and Consumer Goods")]
        RetailConsumerGoods,

        Automotive,

        [Display(Name = "Aerospace and Defense")]
        AerospaceDefense,

        [Display(Name = "Education and E-learning")]
        EducationElearning,

        [Display(Name = "Agriculture and Food Production")]
        AgricultureFoodProduction,

        [Display(Name = "Environmental and Sustainability")]
        EnvironmentalSustainability,

        [Display(Name = "Social Impact and Non-profit")]
        SocialImpactNonProfit,

        [Display(Name = "Biotechnology and Pharmaceuticals")]
        BiotechnologyPharmaceuticals,

        [Display(Name = "Art and Culture")]
        ArtCulture,
        Electrical, Welding, Construction

    }
    public enum eModule
    {
        [Display(Name = "Module 1")]
        Module1,
        [Display(Name = "Module 2")]
        Module2,
        [Display(Name = "Module 3")]
        Module3,
        [Display(Name = "Module 4")]
        Module4,
        [Display(Name = "Module 5")]
        Module5,
        [Display(Name = "Module 6")]
        Module6,
        [Display(Name = "Module 7")]
        Module7,
        [Display(Name = "Module 8")]
        Module8,
        [Display(Name = "Module 9")]
        Module9,
        [Display(Name = "Module 10")]
        Module10,
        [Display(Name = "Module 11")]
        Module11,
        [Display(Name = "Module 12")]
        Module12,
        [Display(Name = "Module 13")]
        Module13,
        [Display(Name = "Module 14")]
        Module14,
        [Display(Name = "Module 15")]
        Module15,
        [Display(Name = "Test 1")]
        Test1,
        [Display(Name = "Test 2")]
        Test2

    }
    public enum eMaterialType
    {
        Assessment,
        [Display(Name = "Learner Material")]
        Material,
        [Display(Name = "Non Academic")]
        NonAcademic,
        Scope,
        [Display(Name = "Past Exam Question Papers")]
        PastPapers,
        Memorandum,Communique
    }
    public enum eTrade
    {
        Welder,Plumber,Electrician, Bricklayer, Painter,
        OHS,
        [Display(Name = "Office Administrator")]
        OfficeAdmin,NATED,
        [Display(Name = "Pest Management Officer")]
        PestManagement,
        Bookkeeper,ECD,
        [Display(Name = "Computer Technician")]
        ComputerTechnician,
        [Display(Name = "Supply Chain Practitioner")]
        SupplyChain,
        [Display(Name = "Project Management")]
        ProjectManagement
    }
    public enum eDepartment
    {
        None,Welding, Plumbing, Electrical,
        ICT, Engineering, ETQA, Training, Corporate,
        Finance,
        [Display(Name = "Health & Safety")]
        Health,
        Admin,Operations,
        [Display(Name = "Environmental Services")]
        Environmental,
        Marketing

    }
    public enum ReportType
    {
        [Display(Name = "Weekly report")]
        Weekly,
        [Display(Name = "Monthly report")]
        Monthly,
        [Display(Name = "Annual report")]
        Annual
    }
    public enum ePhase
    {
        [Display(Name = "Phase 1")]
        Phase1,
        [Display(Name = "Phase 2")]
        Phase2,
        [Display(Name = "Phase 3")]
        Phase3,
        [Display(Name = "Level 1")]
        Level1,
        [Display(Name = "Level 2")]
        Level2,
        [Display(Name = "Level 3")]
        Level3,
        [Display(Name = "Level 4")]
        Level4,
        ARPL,
        [Display(Name = "Not Applicable")]
        N_A
    }
    public enum eUrgency
    {
        High,
        Moderate,
        Low
           
    }
    public enum eOperation 
    {
        [Display(Name = "I'd like to - Create My Report")]
        Create,
        [Display(Name = "I'd like to - Upload My Report")]
        Upload
    }
    public enum eCourseType
    {
        [Display(Name = "Occupational - Trade")]
        OccupationalTrade,
        [Display(Name = "Occupational - Non Trade")]
        OccupationalNonTrade,
        [Display(Name = "Short Skills")]
        ShortSkills,
        [Display(Name = "Nated Studies")]
        Nated
    }
    public enum eNQF
    {
        [Display(Name = "NQF Level 1")]
        NQFLevel1,
        [Display(Name = "NQF Level 2")]
        NQFLevel2,
        [Display(Name = "NQF Level 3")]
        NQFLevel3,
        [Display(Name = "NQF Level 4")]
        NQFLevel4,
        [Display(Name = "NQF Level 5")]
        NQFLevel5,
        [Display(Name = "NQF Level 6")]
        NQFLevel6,

    }
    public enum eDuration
    {
        [Display(Name = "One Week")]
        OneWeek,
        [Display(Name = "Two Weeks")]
        TwoWeeks,
        [Display(Name = "Three Weeks")]
        ThreeWeeks,
        [Display(Name = "One Month")]
        Month
    }
    public enum eFunder
    {
        NSF,
        Mega,
        CETA,
        Silulumanzi,
        Eskom,
        [Display(Name = "Department of Public Works")]
        PublicWorks,
        [Display(Name = "Manganese Metal Company (MMC)")]
        MMC,
        [Display(Name = "Mandlakazi Electrical Technologies")]
        MET,
        EWSETA,
        [Display(Name = "White River Saw Mills")]
        Sawmills,
        [Display(Name = "York Timber")]
        YorkTimber,
        AgriSeta,
        [Display(Name = "Forek Institute of technology")]
        ForekInstitute,
        [Display(Name = "Self-Funded")]
        SelfFunded


    }
    public enum eFunderType
    {
        [Display(Name = "Self-Funded")]
        SelfFunded,
        [Display(Name = "No Funding")]
        NotFunded,
        [Display(Name = "Company-Funded")]
        Company
    }
    public enum eSelection
    {
        Pending,
        [Display(Name = "Yes - I approve")]
        Yes,
        [Display(Name = "No - I Disapprove")]
        No
    }
    public enum eAttempts
    {
        First,Second,Third
    }
    public enum eFileSelection
    {
        [Display(Name = "Subject File")]
        SubjectFile,
        [Display(Name = "Assessment File")]
        AssessmentFile,
        [Display(Name = "Icass Evidence")]
        IcassEvidence
    }
}
