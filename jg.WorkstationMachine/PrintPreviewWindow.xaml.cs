using Spire.Pdf;
using System;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

namespace jg.WorkstationMachine
{
    partial class PrintPreviewWindow : Window
    {
        private delegate void LoadXpsMethod();
        private readonly Object m_data;
        private readonly FlowDocument m_doc;
        public static FlowDocument LoadDocumentAndRender(string strTmplName, Object data, IDocumentRenderer renderer = null)
        {
            FlowDocument doc = (FlowDocument)Application.LoadComponent(new Uri(strTmplName, UriKind.RelativeOrAbsolute));
            doc.PagePadding = new Thickness(50);
            doc.DataContext = data;
            if (renderer != null)
            {
                renderer.Render(doc, data);
            }
            return doc;
        }

        public PrintPreviewWindow(string strTmplName, Object data, IDocumentRenderer renderer = null)
        {
            InitializeComponent();
            m_data = data;
            m_doc = LoadDocumentAndRender(strTmplName, data, renderer);
            Dispatcher.BeginInvoke(new LoadXpsMethod(LoadXps), DispatcherPriority.ApplicationIdle);
        }

        public void LoadXps()
        {

            Model.PageUserModel pageUser = m_data as Model.PageUserModel;

            var pdfPath = System.IO.Directory.GetCurrentDirectory() + "\\data\\" + pageUser.PageName+ "_学习工作页.pdf";
            var xpsPath = System.IO.Directory.GetCurrentDirectory() + "\\data\\xps\\" + pageUser.PageName + "_学习工作页.xps";

            if (!System.IO.File.Exists(xpsPath))
            {
                PdfDocument doc = new PdfDocument();
                doc.LoadFromFile(pdfPath);
                doc.SaveToFile(xpsPath, FileFormat.XPS);
            }
           
            LoadXps(new string[] { xpsPath});      
            
            ////构造一个基于内存的xps document
            //MemoryStream ms = new MemoryStream();
            //Package package = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite);
            //Uri DocumentUri = new Uri("pack://InMemoryDocument.xps");
            //PackageStore.RemovePackage(DocumentUri);
            //PackageStore.AddPackage(DocumentUri, package);
            //XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Fast, DocumentUri.AbsoluteUri);

            ////将flow document写入基于内存的xps document中去
            //XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
            //writer.Write(((IDocumentPaginatorSource)m_doc).DocumentPaginator);

            ////获取这个基于内存的xps document的fixed document
            //docViewer.Document = xpsDocument.GetFixedDocumentSequence();

            ////关闭基于内存的xps document
            //xpsDocument.Close();
        }

        public void LoadXps(string[] list)
        {
            MemoryStream ms2 = new MemoryStream();
            Package container = Package.Open(ms2, FileMode.Create, FileAccess.ReadWrite);
            Uri DocumentUri2 = new Uri("pack://InMemoryDocument2.xps");
            PackageStore.RemovePackage(DocumentUri2);
            PackageStore.AddPackage(DocumentUri2, container);

            XpsDocument xpsDoc = new XpsDocument(container, CompressionOption.Fast, DocumentUri2.AbsoluteUri);
            XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
            FixedDocumentSequence seqNew = new FixedDocumentSequence();

            #region 把FlowDocument放进去
            //构造一个基于内存的xps document
            MemoryStream ms = new MemoryStream();
            Package package = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite);
            Uri DocumentUri = new Uri("pack://InMemoryDocument.xps");
            PackageStore.RemovePackage(DocumentUri);
            PackageStore.AddPackage(DocumentUri, package);
            XpsDocument xpsDocument = new XpsDocument(package, CompressionOption.Fast, DocumentUri.AbsoluteUri);
            //将flow document写入基于内存的xps document中去
            XpsDocumentWriter writer = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
            writer.Write(((IDocumentPaginatorSource)m_doc).DocumentPaginator);

            var doc = xpsDocument.GetFixedDocumentSequence();

            foreach (DocumentReference r in doc.References)
            {
                DocumentReference newRef = new DocumentReference();
                (newRef as IUriContext).BaseUri = (r as IUriContext).BaseUri;
                newRef.Source = r.Source;
                seqNew.References.Add(newRef);
            }

            xpsDocument.Close();

            #endregion

            foreach (string sourceDocument in list)
            {
                XpsDocument xpsOld = new XpsDocument(sourceDocument, FileAccess.Read);
                FixedDocumentSequence seqOld = xpsOld.GetFixedDocumentSequence();
                foreach (DocumentReference r in seqOld.References)
                {
                    DocumentReference newRef = new DocumentReference();
                    (newRef as IUriContext).BaseUri = (r as IUriContext).BaseUri;
                    newRef.Source = r.Source;
                    seqNew.References.Add(newRef);
                }
                xpsOld.Close();
            }

            // NOTE: WORK-AROUND for memory-leak in FixedDocumentSequence!!!
            //    UpdateLayout() must be called at least to one fixedpage!!!
            FixedPage page = (seqNew.DocumentPaginator.GetPage(0).Visual) as FixedPage;
            page.UpdateLayout();

            xpsWriter.Write(seqNew);

            docViewer.Document = xpsDoc.GetFixedDocumentSequence();

            xpsDoc.Close();
        }

        private void GridTitle_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Leave_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
