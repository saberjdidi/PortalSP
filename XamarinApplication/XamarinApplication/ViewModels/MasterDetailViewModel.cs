using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Timers;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinApplication.Helpers;
using XamarinApplication.Models;
using XamarinApplication.Resources;
using XamarinApplication.Views;

namespace XamarinApplication.ViewModels
{
    public class MasterDetailViewModel : BaseViewModel
    {
        #region menu
        ObservableCollection<MenuItems> _items;
        public ObservableCollection<MenuItems> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }
        //Another method with expander
        public void LoadShoppingList()
        {
             var items = new List<MenuItems>
             {
                 new MenuItems { Name = "Patient", Icon = "person_identity", Color = Color.DeepPink,
                     Items = new List<MasterMenu>
                     {
                        new MasterMenu { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientPage) }
                 } },
                 new MenuItems { Name = Languages.Request, Icon = "request", Color = Color.Orange,
                     Items = new List<MasterMenu>
                     {
                         new MasterMenu { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestsPage) },
                         new MasterMenu { MenuName = Languages.DownloadCSV, MenuIcon = "", TargetType = typeof(DownloadCSVPage) },
                         new MasterMenu { MenuName = "Integration Lab", MenuIcon = "", TargetType = typeof(RequestLabLogPage), IsLatest = true }
                     } }
             };
             Items = new ObservableCollection<MenuItems>(items);
              
        }
        #endregion
        #region Attributes
        public INavigation Navigation { get; set; }
        private ObservableCollection<MasterMenu> _menuItems;
        private ObservableCollection<Carousel> _carousel;
        #endregion

        #region Constructors
        public MasterDetailViewModel(INavigation _navigation)
        {
            LoadShoppingList();
            //PopulateMenu();
            GenerateSource();
            GetCarousels();
            GetNotification();
            Navigation = _navigation;
        }
        #endregion

        #region Properties
        public ObservableCollection<MasterMenu> MenuItems
        {
            get
            {
                return _menuItems;
            }
            set
            {
                if (value != null)
                {
                    _menuItems = value;
                    OnPropertyChanged();
                }
            }
        }
        private bool _showHide = false;
        public bool ShowHide
        {
            get => _showHide;
            set
            {
                _showHide = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Carousel> CarouselImage
        {
            get
            {
                return _carousel;
            }
            set
            {
                _carousel = value;
                OnPropertyChanged();
            }
        }

        MasterMenu _selectedMenu;
        public MasterMenu SelectedMenu
        {
            get
            {
                return _selectedMenu;
            }
            set
            {
                if (_selectedMenu != null)
                {
                    _selectedMenu.Selected = false;
                    _selectedMenu.MenuIcon = _selectedMenu.MenuIcon.Substring(0, _selectedMenu.MenuIcon.Length - 6);
                }


                _selectedMenu = value;

                if (_selectedMenu != null)
                {
                    _selectedMenu.Selected = true;
                    _selectedMenu.MenuIcon += "_color";
                    MessagingCenter.Send<MasterMenu>(_selectedMenu, "OpenMenu");
                }
            }
        }
        private User _user = null;
        public User User
        {
            get { return _user; }
            set
            {
                _user = value;
                OnPropertyChanged();
            }
        }
        private Notification _notification = null;
        public Notification Notification
        {
            get { return _notification; }
            set
            {
                _notification = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region Methods
        private ObservableCollection<MenuTreeView> _menuView;
        public ObservableCollection<MenuTreeView> MenuViews
        {
            get
            {
                return _menuView;
            }
            set
            {
                if (value != null)
                {
                    _menuView = value;
                    OnPropertyChanged();
                }
            }
        }
        public void GenerateSource()
        {
            var Username = Settings.Username;
            User = JsonConvert.DeserializeObject<User>(Username);

            var nodeImageInfo = new ObservableCollection<MenuTreeView>();
            Assembly assembly = typeof(MainPage).GetTypeInfo().Assembly;

            if (User.role.authority.Equals("ADMIN_ROLE"))
            {
                var pat = new MenuTreeView() { MenuName = Languages.Patient, MenuIcon = "person_identity" };
                var patient = new MasterMenu() { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientPage) };
                //var shop = new MasterMenu() { MenuName = "Shopping", MenuIcon = "", TargetType = typeof(ShoppingListView) };

                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                //var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestsPage) };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestADMINPage) };
                var csv = new MasterMenu() { MenuName = Languages.DownloadCSV, MenuIcon = "", TargetType = typeof(DownloadCSVPage) };
                var historic = new MasterMenu() { MenuName = Languages.RequestHistoric, MenuIcon = "", TargetType = typeof(RequestHistoricPage) };
                var log = new MasterMenu() { MenuName = Languages.RequestLog, MenuIcon = "", TargetType = typeof(RequestLogPage) };
                var lab = new MasterMenu() { MenuName = Languages.IntegrationLab, MenuIcon = "", TargetType = typeof(RequestLabLogPage) };
                var executionReport = new MasterMenu() { MenuName = Languages.ExecutionReport, MenuIcon = "", TargetType = typeof(ExecutionReportPage) };
                var requestCompilation = new MasterMenu() { MenuName = Languages.RequestCompilation, MenuIcon = "", TargetType = typeof(RequestCompilationPage) };

                var att = new MenuTreeView() { MenuName = Languages.Attachment, MenuIcon = "attachment" };
                var attachment = new MasterMenu() { MenuName = Languages.Attachment, MenuIcon = "", TargetType = typeof(AttachmentsPage) };

                var statistic = new MenuTreeView() { MenuName = Languages.Statistic, MenuIcon = "money" };
                var billing = new MasterMenu() { MenuName = Languages.BillingExtraction, MenuIcon = "", TargetType = typeof(BillingExtractionPage) };
                var genericStatistic = new MasterMenu() { MenuName = Languages.GenericStatistic, MenuIcon = "", TargetType = typeof(GenericStatisticPage) };

                var report = new MenuTreeView() { MenuName = Languages.Report, MenuIcon = "book" };
                var requestModel = new MasterMenu() { MenuName = Languages.RequestModel, MenuIcon = "", TargetType = typeof(RequestModelPage) };
                var requestType = new MasterMenu() { MenuName = Languages.RequestReport, MenuIcon = "", TargetType = typeof(ReportCatalogPage) };
                var requestReport = new MasterMenu() { MenuName = Languages.Report, MenuIcon = "", TargetType = typeof(ReportsPage) };
                var diagnosticTemplate = new MasterMenu() { MenuName = Languages.DiagnosticTemplate, MenuIcon = "", TargetType = typeof(DiagnosticTemplatePage) };
                var postedReport = new MasterMenu() { MenuName = Languages.PostedReport, MenuIcon = "", TargetType = typeof(PostedReportPage) };

                var administration = new MenuTreeView() { MenuName = Languages.Administration, MenuIcon = "administration" };
                var catalog = new MasterMenu() { MenuName = Languages.RequestCatalog, MenuIcon = "", TargetType = typeof(RequestCatalogPage) };
                var branch = new MasterMenu() { MenuName = Languages.Branch, MenuIcon = "", TargetType = typeof(BranchPage) };
                var icdo = new MasterMenu() { MenuName = "ICDO", MenuIcon = "", TargetType = typeof(ICDOPage) };
                var siapec = new MasterMenu() { MenuName = "SIAPEC", MenuIcon = "", TargetType = typeof(SIAPECPage) };
                var snomed = new MasterMenu() { MenuName = "SNOMED", MenuIcon = "", TargetType = typeof(SNOMEDPage) };
                var nomenclature = new MasterMenu() { MenuName = Languages.Nomenclature, MenuIcon = "", TargetType = typeof(NomenclaturePage) };
                var nomenclature_RL = new MasterMenu() { MenuName = Languages.Nomenclature + "_RL", MenuIcon = "", TargetType = typeof(NomenclatureRLPage) };
                var nomenclatureHistoric = new MasterMenu() { MenuName = Languages.NomenclatureHistoric, MenuIcon = "", TargetType = typeof(NomenclatureHistoricPage) };
                var antecedent = new MasterMenu() { MenuName = Languages.Antecedent, MenuIcon = "", TargetType = typeof(AntecedentPage) };
                var illness = new MasterMenu() { MenuName = Languages.IllnessEvent, MenuIcon = "", TargetType = typeof(IllnessEventPage) };
                var checklist = new MasterMenu() { MenuName = Languages.CheckList, MenuIcon = "", TargetType = typeof(CheckListPage) };
                var symptoms = new MasterMenu() { MenuName = "Symptoms", MenuIcon = "", TargetType = typeof(SymptomsPage) };
                var invoiceType = new MasterMenu() { MenuName = Languages.InvoiceType, MenuIcon = "", TargetType = typeof(InvoiceTypePage) };
                var regime = new MasterMenu() { MenuName = Languages.TaxRegime, MenuIcon = "", TargetType = typeof(TaxRegimePage) };
                var payment = new MasterMenu() { MenuName = Languages.Payment, MenuIcon = "", TargetType = typeof(PaymentPage) };
                var tva = new MasterMenu() { MenuName = "TVA", MenuIcon = "", TargetType = typeof(TVAPage) };
                var convention = new MasterMenu() { MenuName = Languages.Convention, MenuIcon = "", TargetType = typeof(ConventionPage) };
                var client = new MasterMenu() { MenuName = Languages.Client, MenuIcon = "", TargetType = typeof(ClientPage) };
                var nazLocal = new MasterMenu() { MenuName = "Naz Local", MenuIcon = "", TargetType = typeof(NazLocalPage) };
                var notification = new MasterMenu() { MenuName = Languages.Notification, MenuIcon = "", TargetType = typeof(NotificationPage) };
                var mailNotification = new MasterMenu() { MenuName = Languages.MailNotification, MenuIcon = "", TargetType = typeof(JobCronPage) };

                var resource = new MenuTreeView() { MenuName = Languages.Resources, MenuIcon = "ressource" };
                var user = new MasterMenu() { MenuName = Languages.User, MenuIcon = "", TargetType = typeof(UserPage) };
                var role = new MasterMenu() { MenuName = Languages.Role, MenuIcon = "", TargetType = typeof(RolePage) };
                var privilege = new MasterMenu() { MenuName = Languages.Privilege, MenuIcon = "", TargetType = typeof(PrivilegePage) };
                var room = new MasterMenu() { MenuName = Languages.Room, MenuIcon = "", TargetType = typeof(RoomPage) };
                var instrument = new MasterMenu() { MenuName = Languages.Instrument, MenuIcon = "", TargetType = typeof(InstrumentPage) };
                var doctor = new MasterMenu() { MenuName = Languages.Doctor, MenuIcon = "", TargetType = typeof(DoctorPage) };
                var service = new MasterMenu() { MenuName = Languages.Service, MenuIcon = "", TargetType = typeof(ServicePage) };
                var uploadCSV = new MasterMenu() { MenuName = Languages.ConfigurationUploadCSV, MenuIcon = "", TargetType = typeof(ConfigurationUploadCSVPage) };
                var serviceDoc = new MasterMenu() { MenuName = Languages.ServiceDocument, MenuIcon = "", TargetType = typeof(ServiceDocumentPage) };
                var docDefenition = new MasterMenu() { MenuName = Languages.DocumentDefinition, MenuIcon = "", TargetType = typeof(DocumentDefinitionPage) };
                var document = new MasterMenu() { MenuName = Languages.Document, MenuIcon = "", TargetType = typeof(DocumentPage) };
                var closureCalendar = new MasterMenu() { MenuName = Languages.ClosureCalendar, MenuIcon = "", TargetType = typeof(ClosureCalendarPage) };

                var invoice = new MenuTreeView() { MenuName = Languages.Invoicing, MenuIcon = "payment" };
                var facture = new MasterMenu() { MenuName = Languages.Invoicing, MenuIcon = "", TargetType = typeof(InvoicePage) };

                var histo = new MenuTreeView() { MenuName = "HistoBox", MenuIcon = "dashboard_color" };
                var histoBox = new MasterMenu() { MenuName = "HistoBox", MenuIcon = "", TargetType = typeof(HistoBoxPage) };

                pat.SubFiles = new ObservableCollection<MasterMenu>
                {
                    patient
                };
                    req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request,
                    csv,
                    log,
                    historic,
                    lab,
                    executionReport,
                    requestCompilation
                };
                    att.SubFiles = new ObservableCollection<MasterMenu>
                {
                    attachment
                };
                    statistic.SubFiles = new ObservableCollection<MasterMenu>
                {
                    billing,
                    genericStatistic
                };
                    report.SubFiles = new ObservableCollection<MasterMenu>
                {
                    requestModel,
                    requestType,
                    requestReport,
                    diagnosticTemplate,
                    postedReport
                };
                    administration.SubFiles = new ObservableCollection<MasterMenu>
                {
                    catalog,
                    branch,
                    icdo,
                    siapec,
                    snomed,
                    nomenclature,
                    nomenclature_RL,
                    nomenclatureHistoric,
                    antecedent,
                    illness,
                    checklist,
                    symptoms,
                    invoiceType,
                    regime,
                    payment,
                    tva,
                    convention,
                    client,
                    nazLocal,
                    notification,
                    mailNotification
                };
                    resource.SubFiles = new ObservableCollection<MasterMenu>
                {
                    user,
                    role,
                    privilege,
                    room,
                    instrument,
                    doctor,
                    service,
                    uploadCSV,
                    serviceDoc,
                    docDefenition,
                    document,
                    closureCalendar
                };
                invoice.SubFiles = new ObservableCollection<MasterMenu>
                {
                    facture
                };
                histo.SubFiles = new ObservableCollection<MasterMenu>
                {
                    histoBox
                };

                nodeImageInfo.Add(pat);
                nodeImageInfo.Add(req);
                nodeImageInfo.Add(att);
                nodeImageInfo.Add(statistic);
                nodeImageInfo.Add(report);
                nodeImageInfo.Add(administration);
                nodeImageInfo.Add(resource);
                nodeImageInfo.Add(invoice);
                nodeImageInfo.Add(histo);
            }
            if (User.role.authority.Equals("OPERATOR_ROLE"))
            {
                var pat = new MenuTreeView() { MenuName = Languages.Patient, MenuIcon = "person_identity" };
                var patient = new MasterMenu() { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientRolePage) };
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestOPERATORPage) };
                var historic = new MasterMenu() { MenuName = Languages.RequestHistoric, MenuIcon = "", TargetType = typeof(RequestHistoricPage) };
                var att = new MenuTreeView() { MenuName = Languages.Attachment, MenuIcon = "attachment" };
                var attachment = new MasterMenu() { MenuName = Languages.Attachment, MenuIcon = "", TargetType = typeof(AttachmentsPage) };
                var statistic = new MenuTreeView() { MenuName = Languages.Statistic, MenuIcon = "money" };
                var billing = new MasterMenu() { MenuName = Languages.BillingExtraction, MenuIcon = "", TargetType = typeof(BillingExtractionPage) };
                pat.SubFiles = new ObservableCollection<MasterMenu>
                {
                    patient
                };
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request,
                    historic
                };
                att.SubFiles = new ObservableCollection<MasterMenu>
                {
                    attachment
                };
                statistic.SubFiles = new ObservableCollection<MasterMenu>
                {
                    billing
                };
                nodeImageInfo.Add(pat);
                nodeImageInfo.Add(req);
                nodeImageInfo.Add(att);
                nodeImageInfo.Add(statistic);
            }
            if (User.role.authority.Equals("TECHNICAL_ROLE"))
            {
                var pat = new MenuTreeView() { MenuName = Languages.Patient, MenuIcon = "person_identity" };
                var patient = new MasterMenu() { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientRolePage) };
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestTECHNICALPage) };
                var csv = new MasterMenu() { MenuName = Languages.DownloadCSV, MenuIcon = "", TargetType = typeof(DownloadCSVPage) };
                var historic = new MasterMenu() { MenuName = Languages.RequestHistoric, MenuIcon = "", TargetType = typeof(RequestHistoricPage) };
                var log = new MasterMenu() { MenuName = Languages.RequestLog, MenuIcon = "", TargetType = typeof(RequestLogPage) };
                pat.SubFiles = new ObservableCollection<MasterMenu>
                {
                    patient
                };
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request,
                    csv,
                    historic,
                    log
                };
                nodeImageInfo.Add(pat);
                nodeImageInfo.Add(req);
            }
            if (User.role.authority.Equals("DOCTOR_ROLE"))
            {
                var pat = new MenuTreeView() { MenuName = Languages.Patient, MenuIcon = "person_identity" };
                var patient = new MasterMenu() { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientRolePage) };
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestDoctorPage) };
                var handledReq = new MasterMenu() { MenuName = Languages.HandledRequest, MenuIcon = "", TargetType = typeof(RequestHandledPage) };
                var csv = new MasterMenu() { MenuName = Languages.DownloadCSV, MenuIcon = "", TargetType = typeof(DownloadCSVPage) };
                var historic = new MasterMenu() { MenuName = Languages.RequestHistoric, MenuIcon = "", TargetType = typeof(RequestHistoricPage) };
                var log = new MasterMenu() { MenuName = Languages.RequestLog, MenuIcon = "", TargetType = typeof(RequestLogPage) };
                var lab = new MasterMenu() { MenuName = Languages.IntegrationLab, MenuIcon = "", TargetType = typeof(RequestLabLogPage) };
                var requestCompilation = new MasterMenu() { MenuName = Languages.RequestCompilation, MenuIcon = "", TargetType = typeof(RequestCompilationPage) };
                var statistic = new MenuTreeView() { MenuName = Languages.Statistic, MenuIcon = "money" };
                var billing = new MasterMenu() { MenuName = Languages.BillingExtraction, MenuIcon = "", TargetType = typeof(BillingExtractionPage) };
                var genericStatistic = new MasterMenu() { MenuName = Languages.GenericStatistic, MenuIcon = "", TargetType = typeof(GenericStatisticPage) };
                var report = new MenuTreeView() { MenuName = Languages.Report, MenuIcon = "book" };
                var diagnosticTemplate = new MasterMenu() { MenuName = Languages.DiagnosticTemplate, MenuIcon = "", TargetType = typeof(DiagnosticTemplatePage) };
                var postedReport = new MasterMenu() { MenuName = Languages.PostedReport, MenuIcon = "", TargetType = typeof(PostedReportPage) };
                var resource = new MenuTreeView() { MenuName = Languages.Resources, MenuIcon = "ressource" };
                var role = new MasterMenu() { MenuName = Languages.Role, MenuIcon = "", TargetType = typeof(RolePage) };
                var closureCalendar = new MasterMenu() { MenuName = Languages.ClosureCalendar, MenuIcon = "", TargetType = typeof(ClosureCalendarPage) };
                var histo = new MenuTreeView() { MenuName = "HistoBox", MenuIcon = "dashboard_color" };
                var histoBox = new MasterMenu() { MenuName = "HistoBox", MenuIcon = "", TargetType = typeof(HistoBoxPage) };
                pat.SubFiles = new ObservableCollection<MasterMenu>
                {
                    patient
                };
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request,
                    handledReq,
                    csv,
                    historic,
                    log,
                    lab,
                    requestCompilation
                };
                statistic.SubFiles = new ObservableCollection<MasterMenu>
                {
                    billing,
                    genericStatistic
                };
                report.SubFiles = new ObservableCollection<MasterMenu>
                {
                    diagnosticTemplate,
                    postedReport
                };
                resource.SubFiles = new ObservableCollection<MasterMenu>
                {
                    role,
                    closureCalendar
                };
                histo.SubFiles = new ObservableCollection<MasterMenu>
                {
                    histoBox
                };
                nodeImageInfo.Add(pat);
                nodeImageInfo.Add(req);
                nodeImageInfo.Add(statistic);
                nodeImageInfo.Add(report);
                nodeImageInfo.Add(resource);
                nodeImageInfo.Add(histo);
            }
            if (User.role.authority.Equals("OBESTETRICIAN_ROLE"))
            {
                var pat = new MenuTreeView() { MenuName = Languages.Patient, MenuIcon = "person_identity" };
                var patient = new MasterMenu() { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientRolePage) };
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestOBESTETRICIANPage) };
                var historic = new MasterMenu() { MenuName = Languages.RequestHistoric, MenuIcon = "", TargetType = typeof(RequestHistoricPage) };
                var resource = new MenuTreeView() { MenuName = Languages.Resources, MenuIcon = "ressource" };
                var closureCalendar = new MasterMenu() { MenuName = Languages.ClosureCalendar, MenuIcon = "", TargetType = typeof(ClosureCalendarPage) };
                pat.SubFiles = new ObservableCollection<MasterMenu>
                {
                    patient
                };
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request,
                    historic
                };
                resource.SubFiles = new ObservableCollection<MasterMenu>
                {
                    closureCalendar
                };
                nodeImageInfo.Add(pat);
                nodeImageInfo.Add(req);
                nodeImageInfo.Add(resource);
            }
            if (User.role.authority.Equals("RADIOLOGIST_ROLE"))
            {
                var pat = new MenuTreeView() { MenuName = Languages.Patient, MenuIcon = "person_identity" };
                var patient = new MasterMenu() { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientRolePage) };
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestOPERATORPage) };
                pat.SubFiles = new ObservableCollection<MasterMenu>
                {
                    patient
                };
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request
                };
                nodeImageInfo.Add(pat);
                nodeImageInfo.Add(req);
            }
            if (User.role.authority.Equals("SECRETARY_ROLE"))
            {
                var pat = new MenuTreeView() { MenuName = Languages.Patient, MenuIcon = "person_identity" };
                var patient = new MasterMenu() { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientRolePage) };
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestOPERATORPage) };
                var historic = new MasterMenu() { MenuName = Languages.RequestHistoric, MenuIcon = "", TargetType = typeof(RequestHistoricPage) };
                var att = new MenuTreeView() { MenuName = Languages.Attachment, MenuIcon = "attachment" };
                var attachment = new MasterMenu() { MenuName = Languages.Attachment, MenuIcon = "", TargetType = typeof(AttachmentsPage) };
                var statistic = new MenuTreeView() { MenuName = Languages.Statistic, MenuIcon = "money" };
                var billing = new MasterMenu() { MenuName = Languages.BillingExtraction, MenuIcon = "", TargetType = typeof(BillingExtractionPage) };
                var genericStatistic = new MasterMenu() { MenuName = Languages.GenericStatistic, MenuIcon = "", TargetType = typeof(GenericStatisticPage) };
                pat.SubFiles = new ObservableCollection<MasterMenu>
                {
                    patient
                };
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request,
                    historic
                };
                att.SubFiles = new ObservableCollection<MasterMenu>
                {
                    attachment
                };
                statistic.SubFiles = new ObservableCollection<MasterMenu>
                {
                    billing,
                    genericStatistic
                };
                nodeImageInfo.Add(pat);
                nodeImageInfo.Add(req);
                nodeImageInfo.Add(att);
                nodeImageInfo.Add(statistic);
            }
            if (User.role.authority.Equals("DOCTORTERME_ROLE"))
            {
                var pat = new MenuTreeView() { MenuName = Languages.Patient, MenuIcon = "person_identity" };
                var patient = new MasterMenu() { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientRolePage) };
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestDOCTORTERMEPage) };
                var handledReq = new MasterMenu() { MenuName = Languages.HandledRequest, MenuIcon = "", TargetType = typeof(RequestHandledPage) };
                var historic = new MasterMenu() { MenuName = Languages.RequestHistoric, MenuIcon = "", TargetType = typeof(RequestHistoricPage) };
                var lab = new MasterMenu() { MenuName = Languages.IntegrationLab, MenuIcon = "", TargetType = typeof(RequestLabLogPage) };
                var requestCompilation = new MasterMenu() { MenuName = Languages.RequestCompilation, MenuIcon = "", TargetType = typeof(RequestCompilationPage) };
                var att = new MenuTreeView() { MenuName = Languages.Attachment, MenuIcon = "attachment" };
                var attachment = new MasterMenu() { MenuName = Languages.Attachment, MenuIcon = "", TargetType = typeof(AttachmentsPage) };
                var histo = new MenuTreeView() { MenuName = "HistoBox", MenuIcon = "dashboard_color" };
                var histoBox = new MasterMenu() { MenuName = "HistoBox", MenuIcon = "", TargetType = typeof(HistoBoxPage) };
                pat.SubFiles = new ObservableCollection<MasterMenu>
                {
                    patient
                };
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request,
                    handledReq,
                    historic,
                    lab,
                    requestCompilation
                };
                att.SubFiles = new ObservableCollection<MasterMenu>
                {
                    attachment
                };
                histo.SubFiles = new ObservableCollection<MasterMenu>
                {
                    histoBox
                };
                nodeImageInfo.Add(pat);
                nodeImageInfo.Add(req);
                nodeImageInfo.Add(att);
                nodeImageInfo.Add(histo);
            }
            if (User.role.authority.Equals("DOCTORACCET_ROLE"))
            {
                var pat = new MenuTreeView() { MenuName = Languages.Patient, MenuIcon = "person_identity" };
                var patient = new MasterMenu() { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientRolePage) };
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestDOCTORACCETPage) };
                var historic = new MasterMenu() { MenuName = Languages.RequestHistoric, MenuIcon = "", TargetType = typeof(RequestHistoricPage) };
                var lab = new MasterMenu() { MenuName = Languages.IntegrationLab, MenuIcon = "", TargetType = typeof(RequestLabLogPage) };
                var report = new MenuTreeView() { MenuName = Languages.Report, MenuIcon = "book" };
                var diagnosticTemplate = new MasterMenu() { MenuName = Languages.DiagnosticTemplate, MenuIcon = "", TargetType = typeof(DiagnosticTemplatePage) };
                pat.SubFiles = new ObservableCollection<MasterMenu>
                {
                    patient
                };
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request,
                    historic,
                    lab
                };
                report.SubFiles = new ObservableCollection<MasterMenu>
                {
                    diagnosticTemplate
                };
                nodeImageInfo.Add(pat);
                nodeImageInfo.Add(req);
                nodeImageInfo.Add(report);
            }
            if (User.role.authority.Equals("DOCTORBS_ROLE"))
            {
                var pat = new MenuTreeView() { MenuName = Languages.Patient, MenuIcon = "person_identity" };
                var patient = new MasterMenu() { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientRolePage) };
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestDOCTORBSPage) };
                //var csv = new MasterMenu() { MenuName = Languages.DownloadCSV, MenuIcon = "", TargetType = typeof(DownloadCSVPage) };
                var historic = new MasterMenu() { MenuName = Languages.RequestHistoric, MenuIcon = "", TargetType = typeof(RequestHistoricPage) };
                //var log = new MasterMenu() { MenuName = Languages.RequestLog, MenuIcon = "", TargetType = typeof(RequestLogPage) };
                var lab = new MasterMenu() { MenuName = Languages.IntegrationLab, MenuIcon = "", TargetType = typeof(RequestLabLogPage) };
                var statistic = new MenuTreeView() { MenuName = Languages.Statistic, MenuIcon = "money" };
                var billing = new MasterMenu() { MenuName = Languages.BillingExtraction, MenuIcon = "", TargetType = typeof(BillingExtractionPage) };
                var genericStatistic = new MasterMenu() { MenuName = Languages.GenericStatistic, MenuIcon = "", TargetType = typeof(GenericStatisticPage) };
                var report = new MenuTreeView() { MenuName = Languages.Report, MenuIcon = "book" };
                var diagnosticTemplate = new MasterMenu() { MenuName = Languages.DiagnosticTemplate, MenuIcon = "", TargetType = typeof(DiagnosticTemplatePage) };
                var postedReport = new MasterMenu() { MenuName = Languages.PostedReport, MenuIcon = "", TargetType = typeof(PostedReportPage) };
                var resource = new MenuTreeView() { MenuName = Languages.Resources, MenuIcon = "ressource" };
                var closureCalendar = new MasterMenu() { MenuName = Languages.ClosureCalendar, MenuIcon = "", TargetType = typeof(ClosureCalendarPage) };
                pat.SubFiles = new ObservableCollection<MasterMenu>
                {
                    patient
                };
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request,
                    //csv,
                    historic,
                    //log,
                    lab
                };
                statistic.SubFiles = new ObservableCollection<MasterMenu>
                {
                    billing,
                    genericStatistic
                };
                report.SubFiles = new ObservableCollection<MasterMenu>
                {
                    diagnosticTemplate,
                    postedReport
                };
                resource.SubFiles = new ObservableCollection<MasterMenu>
                {
                    closureCalendar
                };
                nodeImageInfo.Add(pat);
                nodeImageInfo.Add(req);
                nodeImageInfo.Add(statistic);
                //nodeImageInfo.Add(report);
                nodeImageInfo.Add(resource);
            }
            if (User.role.authority.Equals("DOCTORCLIENT_ROLE"))
            {
                var pat = new MenuTreeView() { MenuName = Languages.Patient, MenuIcon = "person_identity" };
                var patient = new MasterMenu() { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientRolePage) };
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestDoctorClientPage) };
                var historic = new MasterMenu() { MenuName = Languages.RequestHistoric, MenuIcon = "", TargetType = typeof(RequestHistoricPage) };
                var lab = new MasterMenu() { MenuName = Languages.IntegrationLab, MenuIcon = "", TargetType = typeof(RequestLabLogPage) };
                var report = new MenuTreeView() { MenuName = Languages.Report, MenuIcon = "book" };
                var diagnosticTemplate = new MasterMenu() { MenuName = Languages.DiagnosticTemplate, MenuIcon = "", TargetType = typeof(DiagnosticTemplatePage) };
                var invoice = new MenuTreeView() { MenuName = Languages.Invoicing, MenuIcon = "payment" };
                var facture = new MasterMenu() { MenuName = Languages.Invoicing, MenuIcon = "", TargetType = typeof(InvoicePage) };
                pat.SubFiles = new ObservableCollection<MasterMenu>
                {
                    patient
                };
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request,
                    historic,
                    lab
                };
                report.SubFiles = new ObservableCollection<MasterMenu>
                {
                    diagnosticTemplate
                };
                invoice.SubFiles = new ObservableCollection<MasterMenu>
                {
                    facture
                };
                nodeImageInfo.Add(pat);
                nodeImageInfo.Add(req);
                nodeImageInfo.Add(report);
                nodeImageInfo.Add(invoice);
            }
            if (User.role.authority.Equals("DOCTORCTRLNOREF_ROLE"))
            {
                var pat = new MenuTreeView() { MenuName = Languages.Patient, MenuIcon = "person_identity" };
                var patient = new MasterMenu() { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientRolePage) };
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestDOCTORCTRLNOREFPage) };
                var historic = new MasterMenu() { MenuName = Languages.RequestHistoric, MenuIcon = "", TargetType = typeof(RequestHistoricPage) };
                var att = new MenuTreeView() { MenuName = Languages.Attachment, MenuIcon = "attachment" };
                var attachment = new MasterMenu() { MenuName = Languages.Attachment, MenuIcon = "", TargetType = typeof(AttachmentsPage) };
                var statistic = new MenuTreeView() { MenuName = Languages.Statistic, MenuIcon = "money" };
                var billing = new MasterMenu() { MenuName = Languages.BillingExtraction, MenuIcon = "", TargetType = typeof(BillingExtractionPage) };
                var genericStatistic = new MasterMenu() { MenuName = Languages.GenericStatistic, MenuIcon = "", TargetType = typeof(GenericStatisticPage) };
                var report = new MenuTreeView() { MenuName = Languages.Report, MenuIcon = "book" };
                var diagnosticTemplate = new MasterMenu() { MenuName = Languages.DiagnosticTemplate, MenuIcon = "", TargetType = typeof(DiagnosticTemplatePage) };
                pat.SubFiles = new ObservableCollection<MasterMenu>
                {
                    patient
                };
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request,
                    historic
                };
                att.SubFiles = new ObservableCollection<MasterMenu>
                {
                    attachment
                };
                statistic.SubFiles = new ObservableCollection<MasterMenu>
                {
                    billing,
                    genericStatistic
                };
                report.SubFiles = new ObservableCollection<MasterMenu>
                {
                    diagnosticTemplate
                };
                nodeImageInfo.Add(pat);
                nodeImageInfo.Add(req);
                nodeImageInfo.Add(att);
                nodeImageInfo.Add(statistic);
                nodeImageInfo.Add(report);
            }
            if (User.role.authority.Equals("DOCTORNOREF_ROLE"))
            {
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestDOCTORTERMEPage) };
                var att = new MenuTreeView() { MenuName = Languages.Attachment, MenuIcon = "attachment" };
                var attachment = new MasterMenu() { MenuName = Languages.Attachment, MenuIcon = "", TargetType = typeof(AttachmentsPage) };
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request
                };
                att.SubFiles = new ObservableCollection<MasterMenu>
                {
                    attachment
                };
                nodeImageInfo.Add(req);
                nodeImageInfo.Add(att);
            }
            if (User.role.authority.Equals("DOCTORSERV_ROLE"))
            {
                var pat = new MenuTreeView() { MenuName = Languages.Patient, MenuIcon = "person_identity" };
                var patient = new MasterMenu() { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientRolePage) };
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestDOCTORTERMEPage) };
                var historic = new MasterMenu() { MenuName = Languages.RequestHistoric, MenuIcon = "", TargetType = typeof(RequestHistoricPage) };
                pat.SubFiles = new ObservableCollection<MasterMenu>
                {
                    patient
                };
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request,
                    historic
                };
                nodeImageInfo.Add(pat);
                nodeImageInfo.Add(req);
            }
            if (User.role.authority.Equals("GYNELOGIST_ROLE"))
            {
                var pat = new MenuTreeView() { MenuName = Languages.Patient, MenuIcon = "person_identity" };
                var patient = new MasterMenu() { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientRolePage) };
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestGYNELOGISTPage) };
                var lab = new MasterMenu() { MenuName = Languages.IntegrationLab, MenuIcon = "", TargetType = typeof(RequestLabLogPage) };
                var report = new MenuTreeView() { MenuName = Languages.Report, MenuIcon = "book" };
                var diagnosticTemplate = new MasterMenu() { MenuName = Languages.DiagnosticTemplate, MenuIcon = "", TargetType = typeof(DiagnosticTemplatePage) };
                var postedReport = new MasterMenu() { MenuName = Languages.PostedReport, MenuIcon = "", TargetType = typeof(PostedReportPage) };
                var resource = new MenuTreeView() { MenuName = Languages.Resources, MenuIcon = "ressource" };
                var closureCalendar = new MasterMenu() { MenuName = Languages.ClosureCalendar, MenuIcon = "", TargetType = typeof(ClosureCalendarPage) };
                pat.SubFiles = new ObservableCollection<MasterMenu>
                {
                    patient
                };
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request,
                    lab
                };
                report.SubFiles = new ObservableCollection<MasterMenu>
                {
                    diagnosticTemplate,
                    postedReport
                };
                resource.SubFiles = new ObservableCollection<MasterMenu>
                {
                    closureCalendar
                };
                nodeImageInfo.Add(pat);
                nodeImageInfo.Add(req);
                nodeImageInfo.Add(report);
                nodeImageInfo.Add(resource);
            }
            if (User.role.authority.Equals("CITOLOGO_ROLE"))
            {
                var pat = new MenuTreeView() { MenuName = Languages.Patient, MenuIcon = "person_identity" };
                var patient = new MasterMenu() { MenuName = Languages.Patient, MenuIcon = "", TargetType = typeof(PatientRolePage) };
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestTECHNICALPage) };
                pat.SubFiles = new ObservableCollection<MasterMenu>
                {
                    patient
                };
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request
                };
                nodeImageInfo.Add(pat);
                nodeImageInfo.Add(req);
            }
            if (User.role.authority.Equals("AMBULATORY_ROLE"))
            {
                var req = new MenuTreeView() { MenuName = Languages.Request, MenuIcon = "request" };
                var request = new MasterMenu() { MenuName = Languages.Request, MenuIcon = "", TargetType = typeof(RequestAMBULATORYPage) };
                
                req.SubFiles = new ObservableCollection<MasterMenu>
                {
                    request
                };
                nodeImageInfo.Add(req);
            }
            if (User.role.authority.Equals("VISITOR_ROLE"))
            {
                var general = new MenuTreeView() { MenuName = "Generale", MenuIcon = "setting" };
                var catalog = new MasterMenu() { MenuName = "Catalogo", MenuIcon = "", TargetType = typeof(NomenclaturePage) };

                general.SubFiles = new ObservableCollection<MasterMenu>
                {
                    catalog
                };
                nodeImageInfo.Add(general);
            }

            MenuViews = nodeImageInfo;
        }
        public void PopulateMenu()
        {
           var Username = Settings.Username;
            User = JsonConvert.DeserializeObject<User>(Username);

                MenuItems = new ObservableCollection<MasterMenu>();
            if (User.role.authority.Equals("ADMIN_ROLE"))
            {
                MenuItems.Add(new MasterMenu { MenuName = Languages.Request, MenuIcon = "request", TargetType = typeof(RequestsPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Patient, MenuIcon = "person_identity", TargetType = typeof(PatientPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.DownloadCSV, MenuIcon = "ic_menu_csv", TargetType = typeof(DownloadCSVPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Report, MenuIcon = "ic_menu_posted_reports", TargetType = typeof(ReportsPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Attachment, MenuIcon = "attachment", TargetType = typeof(AttachmentsPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Branch, MenuIcon = "", TargetType = typeof(BranchPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.RequestCatalog, MenuIcon = "", TargetType = typeof(RequestCatalogPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.User, MenuIcon = "", TargetType = typeof(UserPage) });
                MenuItems.Add(new MasterMenu { MenuName = "ICDO", MenuIcon = "", TargetType = typeof(ICDOPage) });
                MenuItems.Add(new MasterMenu { MenuName = "SIAPEC", MenuIcon = "", TargetType = typeof(SIAPECPage) });
                MenuItems.Add(new MasterMenu { MenuName = "SNOMED", MenuIcon = "", TargetType = typeof(SNOMEDPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Service", MenuIcon = "", TargetType = typeof(ServicePage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Doctor, MenuIcon = "", TargetType = typeof(DoctorPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Client, MenuIcon = "", TargetType = typeof(ClientPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Nomenclature, MenuIcon = "", TargetType = typeof(NomenclaturePage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Nomenclature+"_RL", MenuIcon = "", TargetType = typeof(NomenclatureRLPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Request Historic", MenuIcon = "", TargetType = typeof(RequestHistoricPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Request Log", MenuIcon = "", TargetType = typeof(RequestLogPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Integration Lab", MenuIcon = "", TargetType = typeof(RequestLabLogPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.DocumentDefinition, MenuIcon = "", TargetType = typeof(DocumentDefinitionPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Closure Calendar", MenuIcon = "", TargetType = typeof(ClosureCalendarPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Diagnostic Template", MenuIcon = "", TargetType = typeof(DiagnosticTemplatePage) });
                MenuItems.Add(new MasterMenu { MenuName = "Request Model", MenuIcon = "", TargetType = typeof(RequestModelPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Request Type", MenuIcon = "", TargetType = typeof(ReportCatalogPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Billing Extraction", MenuIcon = "", TargetType = typeof(BillingExtractionPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Invoice Type", MenuIcon = "", TargetType = typeof(InvoiceTypePage) });
                MenuItems.Add(new MasterMenu { MenuName = "CheckList", MenuIcon = "", TargetType = typeof(CheckListPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Payment", MenuIcon = "", TargetType = typeof(PaymentPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Tax Regime", MenuIcon = "", TargetType = typeof(TaxRegimePage) });
                MenuItems.Add(new MasterMenu { MenuName = "TVA", MenuIcon = "", TargetType = typeof(TVAPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Convention", MenuIcon = "", TargetType = typeof(ConventionPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Naz Local", MenuIcon = "", TargetType = typeof(NazLocalPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Antecedent", MenuIcon = "", TargetType = typeof(AntecedentPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Illness Event", MenuIcon = "", TargetType = typeof(IllnessEventPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Service Document", MenuIcon = "", TargetType = typeof(ServiceDocumentPage) });
            }
            if (User.role.authority.Equals("OPERATOR_ROLE"))
            {
                MenuItems.Add(new MasterMenu { MenuName = Languages.Patient, MenuIcon = "person_identity", TargetType = typeof(PatientPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Request, MenuIcon = "request", TargetType = typeof(RequestsPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Attachment, MenuIcon = "attachment", TargetType = typeof(AttachmentsPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Request Historic", MenuIcon = "", TargetType = typeof(RequestHistoricPage) });
            }
            if (User.role.authority.Equals("TECHNICAL_ROLE"))
            {
               // MenuItems.Add(new MasterMenu { MenuName = Languages.Patient, MenuIcon = "person_identity", TargetType = typeof(PatientPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Request, MenuIcon = "request", TargetType = typeof(RequestsPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.DownloadCSV, MenuIcon = "ic_menu_csv", TargetType = typeof(DownloadCSVPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Request Historic", MenuIcon = "", TargetType = typeof(RequestHistoricPage) });
            }
            if (User.role.authority.Equals("DOCTOR_ROLE"))
            {
                MenuItems.Add(new MasterMenu { MenuName = Languages.Patient, MenuIcon = "person_identity", TargetType = typeof(PatientPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Request, MenuIcon = "request", TargetType = typeof(RequestsPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.DownloadCSV, MenuIcon = "ic_menu_csv", TargetType = typeof(DownloadCSVPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Request Historic", MenuIcon = "", TargetType = typeof(RequestHistoricPage) });
            }
            if (User.role.authority.Equals("OBESTETRICIAN_ROLE"))
            {
                MenuItems.Add(new MasterMenu { MenuName = Languages.Patient, MenuIcon = "person_identity", TargetType = typeof(PatientPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Request, MenuIcon = "request", TargetType = typeof(RequestsPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Request Historic", MenuIcon = "", TargetType = typeof(RequestHistoricPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Closure Calendar", MenuIcon = "", TargetType = typeof(ClosureCalendarPage) });
            }
            if (User.role.authority.Equals("RADIOLOGIST_ROLE"))
            {
                MenuItems.Add(new MasterMenu { MenuName = Languages.Patient, MenuIcon = "person_identity", TargetType = typeof(PatientPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Request, MenuIcon = "request", TargetType = typeof(RequestsPage) });
            }
            if (User.role.authority.Equals("SECRETARY_ROLE"))
            {
                MenuItems.Add(new MasterMenu { MenuName = Languages.Patient, MenuIcon = "person_identity", TargetType = typeof(PatientPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Request, MenuIcon = "request", TargetType = typeof(RequestsPage) });
                MenuItems.Add(new MasterMenu { MenuName = "Request Historic", MenuIcon = "", TargetType = typeof(RequestHistoricPage) });
                MenuItems.Add(new MasterMenu { MenuName = Languages.Attachment, MenuIcon = "attachment", TargetType = typeof(AttachmentsPage) });
            }

        }

        public void GetCarousels()
        {
            CarouselImage = new ObservableCollection<Carousel>();
            CarouselImage.Add(new Carousel { Heading = "SmartPath", Message = "SmartPath", Caption = "SmartPath", Image = "logo" });
            CarouselImage.Add(new Carousel { Heading = "SmartPath", Message = "SmartPath", Caption = "SmartPath", Image = "microscope" });
            CarouselImage.Add(new Carousel { Heading = "SmartPath", Message = "SmartPath", Caption = "SmartPath", Image = "microscope1" });
            CarouselImage.Add(new Carousel { Heading = "SmartPath", Message = "SmartPath", Caption = "SmartPath", Image = "ic_branch" });
            CarouselImage.Add(new Carousel { Heading = "SmartPath", Message = "SmartPath", Caption = "SmartPath", Image = "user" });

        }
        public async void GetNotification()
        {
                var cookie = Settings.Cookie;
                var res = cookie.Substring(11, 32);
                var cookieContainer = new CookieContainer();
                var handler = new HttpClientHandler() { CookieContainer = cookieContainer };
                var client = new HttpClient(handler);
                var url = "https://portalesp.smart-path.it/Portalesp/notification/getActiveNotify";
                Debug.WriteLine("********url*************");
                Debug.WriteLine(url);
                client.BaseAddress = new Uri(url);
                cookieContainer.Add(client.BaseAddress, new Cookie("JSESSIONID", res));
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", response.StatusCode.ToString(), "ok");
                    return;
                }
                var result = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("********result*************");
                Debug.WriteLine(result);
                var json = JsonConvert.DeserializeObject<Notification>(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                Debug.WriteLine("********json*************");
                Debug.WriteLine(json);
            Notification = (Notification)json;
            if(Notification.message.Equals("not found"))
            {

            }
            else
            {
            DependencyService.Get<INotification>().CreateNotification("PortalSP", Notification.message);
            }
        }
        #endregion

        #region Commands
        public ICommand OpenAdministrationCommand
        {
            get
            {
                return new Command(() =>
                {
                    MenuItems = new ObservableCollection<MasterMenu>();
                    MenuItems.Add(new MasterMenu { MenuName = "Branch", MenuIcon = "", TargetType = typeof(BranchPage) });
                    MenuItems.Add(new MasterMenu { MenuName = "ICDO", MenuIcon = "", TargetType = typeof(ICDOPage) });
                    MenuItems.Add(new MasterMenu { MenuName = "SIAPEC", MenuIcon = "", TargetType = typeof(SIAPECPage) });
                    ShowHide = true;
                });
            }
        }

        /*public ICommand LogoutCommand
        {
            get
            {
                return new Command(async() =>
                {
                    
                    var confirm = Application.Current.MainPage.DisplayAlert("Exit", "Do you wan't to exit the App ?", "Yes", "No");
                    if (confirm.Equals("Yes"))
                    {
                        Settings.AccessToken = string.Empty;
                        Debug.WriteLine(Settings.Username);
                        Settings.Username = string.Empty;
                        Debug.WriteLine(Settings.Password);
                        Settings.Password = string.Empty;

                        await Navigation.PushModalAsync(new LoginPage());
                    }
                    else if(confirm.Equals("No"))
                    {
                        await Navigation.PushModalAsync(new MainPage());
                    }
                });
            }
        }*/
        #endregion
    }
}
