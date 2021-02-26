using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using XamarinApplication.Interfaces;
using XamarinApplication.Resources;

namespace XamarinApplication.Helpers
{
    public static class Languages
    {
        static Languages()
        {
            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            Resource.Culture = ci;
            DependencyService.Get<ILocalize>().SetLocale(ci);
        }
        public static string Error
        {
            get { return Resource.Error; }
        }
        public static string Ok
        {
            get { return Resource.Ok; }
        }
        public static string UsernameValidation
        {
            get { return Resource.UsernameValidation; }
        }
        public static string PasswordValidation
        {
            get { return Resource.PasswordValidation; }
        }
        public static string Username
        {
            get { return Resource.Username; }
        }
        public static string Password
        {
            get { return Resource.Password; }
        }
        public static string Warning
        {
            get { return Resource.Warning; }
        }
        public static string CheckConnection
        {
            get { return Resource.CheckConnection; }
        }
        public static string FailedLogin
        {
            get { return Resource.FailedLogin; }
        }
        public static string Exit
        {
            get { return Resource.Exit; }
        }
        public static string Yes
        {
            get { return Resource.Yes; }
        }
        public static string No
        {
            get { return Resource.No; }
        }
        public static string User
        {
            get { return Resource.User; }
        }
        public static string Branch
        {
            get { return Resource.Branch; }
        }
        public static string Serie
        {
            get { return Resource.Serie; }
        }
        public static string Report
        {
            get { return Resource.Report; }
        }
        public static string Nomenclature
        {
            get { return Resource.Nomenclature; }
        }
        public static string Exercice
        {
            get { return Resource.Exercice; }
        }
        public static string Doctor
        {
            get { return Resource.Doctor; }
        }
        public static string TitleMainPage
        {
            get { return Resource.TitleMainPage; }
        }
        public static string DownloadCSV
        {
            get { return Resource.DownloadCSV; }
        }
        public static string NewBranch
        {
            get { return Resource.NewBranch; }
        }
        public static string Logout
        {
            get { return Resource.Logout; }
        }
        public static string Date
        {
            get { return Resource.Date; }
        }
        public static string Informations
        {
            get { return Resource.Informations; }
        }
        public static string Exercices
        {
            get { return Resource.Exercices; }
        }
        public static string Save
        {
            get { return Resource.Save; }
        }
        public static string Patient
        {
            get { return Resource.Patient; }
        }
        public static string Code
        {
            get { return Resource.Code; }
        }
        public static string Name
        {
            get { return Resource.Name; }
        }
        public static string Description
        {
            get { return Resource.Description; }
        }
        public static string Client
        {
            get { return Resource.Client; }
        }
        public static string NewDoctor
        {
            get { return Resource.NewDoctor; }
        }
        public static string Close
        {
            get { return Resource.Close; }
        }
        public static string DocumentDefinition
        {
            get { return Resource.DocumentDefinition; }
        }
        public static string RequestCatalog
        {
            get { return Resource.RequestCatalog; }
        }
        public static string StartDate
        {
            get { return Resource.StartDate; }
        }
        public static string EndDate
        {
            get { return Resource.EndDate; }
        }
        public static string Search
        {
            get { return Resource.Search; }
        }
        public static string NoResult
        {
            get { return Resource.NoResult; }
        }
        public static string Attachment
        {
            get { return Resource.Attachment; }
        }
        public static string Request
        {
            get { return Resource.Request; }
        }
        public static string FirstName
        {
            get { return Resource.FirstName; }
        }
        public static string LastName
        {
            get { return Resource.LastName; }
        }
        public static string FiscalCode
        {
            get { return Resource.FiscalCode; }
        }
        public static string Phone
        {
            get { return Resource.Phone; }
        }
        public static string BirthDate
        {
            get { return Resource.BirthDate; }
        }
        public static string Residence
        {
            get { return Resource.Residence; }
        }
        public static string NoResultsFound
        {
            get { return Resource.NoResultsFound; }
        }
        public static string NewPatient
        {
            get { return Resource.NewPatient; }
        }
        public static string Title
        {
            get { return Resource.Title; }
        }
        public static string PlaceOfBirth
        {
            get { return Resource.PlaceOfBirth; }
        }
        public static string Note
        {
            get { return Resource.Note; }
        }
        public static string Domicile
        {
            get { return Resource.Domicile; }
        }
        public static string BillingAddress
        {
            get { return Resource.BillingAddress; }
        }
        public static string ConditionPayment
        {
            get { return Resource.ConditionPayment; }
        }
        public static string NotePaymentCondition
        {
            get { return Resource.NotePaymentCondition; }
        }
        public static string NewRequestCatalog
        {
            get { return Resource.NewRequestCatalog; }
        }
        public static string BranchExam
        {
            get { return Resource.BranchExam; }
        }
        public static string Topografic
        {
            get { return Resource.Topografic; }
        }
        public static string Intervention
        {
            get { return Resource.Intervention; }
        }
        public static string Procedure
        {
            get { return Resource.Procedure; }
        }
        public static string Valid
        {
            get { return Resource.Valid; }
        }
        public static string NewRequest
        {
            get { return Resource.NewRequest; }
        }
        public static string Operator
        {
            get { return Resource.Operator; }
        }
        public static string Room
        {
            get { return Resource.Room; }
        }
        public static string Instrument
        {
            get { return Resource.Instrument; }
        }
        public static string NewService
        {
            get { return Resource.NewService; }
        }
        public static string ZipCode
        {
            get { return Resource.ZipCode; }
        }
        public static string TVACode
        {
            get { return Resource.TVACode; }
        }
        public static string NewPRESTATION
        {
            get { return Resource.NewPRESTATION; }
        }
        public static string ExamType
        {
            get { return Resource.ExamType; }
        }
        public static string NewUser
        {
            get { return Resource.NewUser; }
        }
        public static string Type
        {
            get { return Resource.Type; }
        }
        public static string Role
        {
            get { return Resource.Role; }
        }
        public static string Enable
        {
            get { return Resource.Enable; }
        }
        public static string EditDoctor
        {
            get { return Resource.EditDoctor; }
        }
        public static string Update
        {
            get { return Resource.Update; }
        }
        public static string EditTOPOGRFIC
        {
            get { return Resource.EditTOPOGRFIC; }
        }
        public static string NewTOPOGRFIC
        {
            get { return Resource.NewTOPOGRFIC; }
        }
        public static string EditPatient
        {
            get { return Resource.EditPatient; }
        }
        public static string EditRequestCatalog
        {
            get { return Resource.EditRequestCatalog; }
        }
        public static string UpdateRequest
        {
            get { return Resource.UpdateRequest; }
        }
        public static string RequestNum
        {
            get { return Resource.RequestNum; }
        }
        public static string EditService
        {
            get { return Resource.EditService; }
        }
        public static string EditPRESTATION
        {
            get { return Resource.EditPRESTATION; }
        }
        public static string EditUser
        {
            get { return Resource.EditUser; }
        }
        public static string From
        {
            get { return Resource.From; }
        }
        public static string To
        {
            get { return Resource.To; }
        }
        public static string Status
        {
            get { return Resource.Status; }
        }
        public static string CodeDescription
        {
            get { return Resource.CodeDescription; }
        }
        public static string ConsentDocument
        {
            get { return Resource.ConsentDocument; }
        }
        public static string DeleteRequest
        {
            get { return Resource.DeleteRequest; }
        }
        public static string Reason
        {
            get { return Resource.Reason; }
        }
        public static string Delete
        {
            get { return Resource.Delete; }
        }
        public static string DoctorDetails
        {
            get { return Resource.DoctorDetails; }
        }
        public static string DocumentDefintionDetails
        {
            get { return Resource.DocumentDefintionDetails; }
        }
        public static string DocumentType
        {
            get { return Resource.DocumentType; }
        }
        public static string DocumentVersion
        {
            get { return Resource.DocumentVersion; }
        }
        public static string NomenclatureDetails
        {
            get { return Resource.NomenclatureDetails; }
        }
        public static string ExecutionRules
        {
            get { return Resource.ExecutionRules; }
        }
        public static string StartValidation
        {
            get { return Resource.StartValidation; }
        }
        public static string EndValidation
        {
            get { return Resource.EndValidation; }
        }
        public static string PatientDetails
        {
            get { return Resource.PatientDetails; }
        }
        public static string FullName
        {
            get { return Resource.FullName; }
        }
        public static string Action
        {
            get { return Resource.Action; }
        }
        public static string PatientSlave
        {
            get { return Resource.PatientSlave; }
        }
        public static string RequestCatalogDetails
        {
            get { return Resource.RequestCatalogDetails; }
        }
        public static string Details
        {
            get { return Resource.Details; }
        }
        public static string ServiceDetails
        {
            get { return Resource.ServiceDetails; }
        }
        public static string RequestDetails
        {
            get { return Resource.RequestDetails; }
        }
        public static string ConfirmationDelete
        {
            get { return Resource.ConfirmationDelete; }
        }
        public static string Confirm
        {
            get { return Resource.Confirm; }
        }
        public static string RequestHistoric
        {
            get { return Resource.RequestHistoric; }
        }
        public static string RequestLog
        {
            get { return Resource.RequestLog; }
        }
        public static string IntegrationLab
        {
            get { return Resource.IntegrationLab; }
        }
        public static string ExecutionReport
        {
            get { return Resource.ExecutionReport; }
        }
        public static string Statistic
        {
            get { return Resource.Statistic; }
        }
        public static string BillingExtraction
        {
            get { return Resource.BillingExtraction; }
        }
        public static string GenericStatistic
        {
            get { return Resource.GenericStatistic; }
        }
        public static string RequestModel
        {
            get { return Resource.RequestModel; }
        }
        public static string RequestReport
        {
            get { return Resource.RequestReport; }
        }
        public static string DiagnosticTemplate
        {
            get { return Resource.DiagnosticTemplate; }
        }
        public static string PostedReport
        {
            get { return Resource.PostedReport; }
        }
        public static string Administration
        {
            get { return Resource.Administration; }
        }
        public static string Language
        {
            get { return Resource.Language; }
        }
        public static string Antecedent
        {
            get { return Resource.Antecedent; }
        }
        public static string IllnessEvent
        {
            get { return Resource.IllnessEvent; }
        }
        public static string CheckList
        {
            get { return Resource.CheckList; }
        }
        public static string InvoiceType
        {
            get { return Resource.InvoiceType; }
        }
        public static string TaxRegime
        {
            get { return Resource.TaxRegime; }
        }
        public static string Payment
        {
            get { return Resource.Payment; }
        }
        public static string Convention
        {
            get { return Resource.Convention; }
        }
        public static string Resources
        {
            get { return Resource.Resources; }
        }
        public static string Service
        {
            get { return Resource.Service; }
        }
        public static string ServiceDocument
        {
            get { return Resource.ServiceDocument; }
        }
        public static string ClosureCalendar
        {
            get { return Resource.ClosureCalendar; }
        }
        public static string StreetDomicile
        {
            get { return Resource.StreetDomicile; }
        }
        public static string StreetResidence
        {
            get { return Resource.StreetResidence; }
        }
        public static string NewRagService
        {
            get { return Resource.NewRagService; }
        }
        public static string EditRagService
        {
            get { return Resource.EditRagService; }
        }
        public static string Exam
        {
            get { return Resource.Exam; }
        }
        public static string NewReport
        {
            get { return Resource.NewReport; }
        }
        public static string EditReport
        {
            get { return Resource.EditReport; }
        }
        public static string NewTemplate
        {
            get { return Resource.NewTemplate; }
        }
        public static string EditTemplate
        {
            get { return Resource.EditTemplate; }
        }
        public static string NewProcedureRL
        {
            get { return Resource.NewProcedureRL; }
        }
        public static string EditProcedureRL
        {
            get { return Resource.EditProcedureRL; }
        }
        public static string NewAntecedent
        {
            get { return Resource.NewAntecedent; }
        }
        public static string EditAntecedent
        {
            get { return Resource.EditAntecedent; }
        }
        public static string NewIllnessEvent
        {
            get { return Resource.NewIllnessEvent; }
        }
        public static string EditIllnessEvent
        {
            get { return Resource.EditIllnessEvent; }
        }
        public static string NewCheckList
        {
            get { return Resource.NewCheckList; }
        }
        public static string EditCheckList
        {
            get { return Resource.EditCheckList; }
        }
        public static string NewInvoiceType
        {
            get { return Resource.NewInvoiceType; }
        }
        public static string EditInvoiceType
        {
            get { return Resource.EditInvoiceType; }
        }
        public static string NewTaxRegime
        {
            get { return Resource.NewTaxRegime; }
        }
        public static string EditTaxRegime
        {
            get { return Resource.EditTaxRegime; }
        }
        public static string NewPayment
        {
            get { return Resource.NewPayment; }
        }
        public static string EditPayment
        {
            get { return Resource.EditPayment; }
        }
        public static string NewTVA
        {
            get { return Resource.NewTVA; }
        }
        public static string EditTVA
        {
            get { return Resource.EditTVA; }
        }
        public static string SocialReason
        {
            get { return Resource.SocialReason; }
        }
        public static string NewConvention
        {
            get { return Resource.NewConvention; }
        }
        public static string EditConvention
        {
            get { return Resource.EditConvention; }
        }
        public static string NewNazLocal
        {
            get { return Resource.NewNazLocal; }
        }
        public static string EditNazLocal
        {
            get { return Resource.EditNazLocal; }
        }
        public static string GlobalConfiguration
        {
            get { return Resource.GlobalConfiguration; }
        }
        public static string NewRoom
        {
            get { return Resource.NewRoom; }
        }
        public static string EditRoom
        {
            get { return Resource.EditRoom; }
        }
        public static string NewInstrument
        {
            get { return Resource.NewInstrument; }
        }
        public static string EditInstrument
        {
            get { return Resource.EditInstrument; }
        }
        public static string NewServiceDocument
        {
            get { return Resource.NewServiceDocument; }
        }
        public static string EditServiceDocument
        {
            get { return Resource.EditServiceDocument; }
        }
        public static string ServiceName
        {
            get { return Resource.ServiceName; }
        }
        public static string ServiceURL
        {
            get { return Resource.ServiceURL; }
        }
        public static string NewClosureCalendar
        {
            get { return Resource.NewClosureCalendar; }
        }
        public static string EditClosureCalendar
        {
            get { return Resource.EditClosureCalendar; }
        }
        public static string Period
        {
            get { return Resource.Period; }
        }
        public static string RequestExam
        {
            get { return Resource.RequestExam; }
        }
        public static string PrintAcceptation
        {
            get { return Resource.PrintAcceptation; }
        }
        public static string NotePatient
        {
            get { return Resource.NotePatient; }
        }
        public static string BiologicalMaterials
        {
            get { return Resource.BiologicalMaterials; }
        }
        public static string Requests
        {
            get { return Resource.Requests; }
        }
        public static string Requestof
        {
            get { return Resource.Requestof; }
        }
        public static string Male
        {
            get { return Resource.Male; }
        }
        public static string Female
        {
            get { return Resource.Female; }
        }
        public static string ALL
        {
            get { return Resource.ALL; }
        }
        public static string Checked
        {
            get { return Resource.Checked; }
        }
        public static string Saved
        {
            get { return Resource.Saved; }
        }
        public static string Sent
        {
            get { return Resource.Sent; }
        }
        public static string ToBeCompleted
        {
            get { return Resource.ToBeCompleted; }
        }
        public static string Validated
        {
            get { return Resource.Validated; }
        }
        public static string Signed
        {
            get { return Resource.Signed; }
        }
        public static string NonSelected
        {
            get { return Resource.NonSelected; }
        }
        public static string None
        {
            get { return Resource.None; }
        }
        public static string HandledRequest
        {
            get { return Resource.HandledRequest; }
        }
        public static string MaxResult
        {
            get { return Resource.MaxResult; }
        }
        public static string Document
        {
            get { return Resource.Document; }
        }
        public static string NomenclatureHistoric
        {
            get { return Resource.NomenclatureHistoric; }
        }
        public static string Privilege
        {
            get { return Resource.Privilege; }
        }
        public static string PreliminaryReport
        {
            get { return Resource.PreliminaryReport; }
        }
        public static string CreatedBy
        {
            get { return Resource.CreatedBy; }
        }
        public static string CreationDate
        {
            get { return Resource.CreationDate; }
        }
        public static string ReportingTime
        {
            get { return Resource.ReportingTime; }
        }
        public static string SamplingDate
        {
            get { return Resource.SamplingDate; }
        }
        public static string Notification
        {
            get { return Resource.Notification; }
        }
        public static string NewNotification
        {
            get { return Resource.NewNotification; }
        }
        public static string EditNotification
        {
            get { return Resource.EditNotification; }
        }
        public static string Message
        {
            get { return Resource.Message; }
        }
        public static string Archive
        {
            get { return Resource.Archive; }
        }
        public static string MailNotification
        {
            get { return Resource.MailNotification; }
        }
        public static string ConfigureEmailAddress
        {
            get { return Resource.ConfigureEmailAddress; }
        }
        public static string Hour
        {
            get { return Resource.Hour; }
        }
        public static string Minute
        {
            get { return Resource.Minute; }
        }
        public static string Day
        {
            get { return Resource.Day; }
        }
        public static string Monday
        {
            get { return Resource.Monday; }
        }
        public static string Tuesday
        {
            get { return Resource.Tuesday; }
        }
        public static string Wednesday
        {
            get { return Resource.Wednesday; }
        }
        public static string Thursday
        {
            get { return Resource.Thursday; }
        }
        public static string Friday
        {
            get { return Resource.Friday; }
        }
        public static string Saturday
        {
            get { return Resource.Saturday; }
        }
        public static string Sunday
        {
            get { return Resource.Sunday; }
        }
        public static string Invoicing
        {
            get { return Resource.Invoicing; }
        }
        public static string ExistingTherapy
        {
            get { return Resource.ExistingTherapy; }
        }
        public static string Price
        {
            get { return Resource.Price; }
        }
        public static string ConfigurationUploadCSV
        {
            get { return Resource.ConfigurationUploadCSV; }
        }
        public static string RequestCompilation
        {
            get { return Resource.RequestCompilation; }
        }
        public static string CancelTackingCharge
        {
            get { return Resource.CancelTackingCharge; }
        }
        public static string CkeckEmail
        {
            get { return Resource.CkeckEmail; }
        }
        public static string EnterEmailAddress
        {
            get { return Resource.EnterEmailAddress; }
        }
        public static string Send
        {
            get { return Resource.Send; }
        }
        public static string Forgotpassword
        {
            get { return Resource.Forgotpassword; }
        }
        public static string SignIn
        {
            get { return Resource.SignIn; }
        }
    }
}
