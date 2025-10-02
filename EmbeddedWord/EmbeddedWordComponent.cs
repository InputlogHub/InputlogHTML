using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DSOFramer;

//[assembly: Guid("F9457B80-4073-45BF-A34D-A8577BF920C0")]
//[assembly: System.Reflection.AssemblyVersion("1.0.0.0")]
//[assembly: System.Windows.Forms.AxHost.TypeLibraryTimeStamp("14/12/2001 15:59:22")]
//[assembly: PrimaryInteropAssembly(1, 1)]

namespace EmbeddedWord
{
    [Clsid("{00460182-9e5e-11d5-b7c8-b8269041dd57}")]
    [DesignTimeVisible(true)]
    [DefaultEvent("OnFileCommand")]
    //[Guid("5F0B1FE6-3ADD-4EA0-8F62-219DED4C7DF4")]
    public class EmbeddedWordComponent : AxHost
    {
        private ConnectionPointCookie Cookie;
        private AxFramerControlEventMulticaster EventMulticaster;
        private _FramerControl Ocx;

        public EmbeddedWordComponent() :
            base("00460182-9e5e-11d5-b7c8-b8269041dd57")
        {
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(65538)]
        public virtual object ActiveDocument
        {
            get
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("ActiveDocument", ActiveXInvokeKind.PropertyGet);
                }
                return Ocx.ActiveDocument;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(65546)]
        public virtual string Caption
        {
            get
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("Caption", ActiveXInvokeKind.PropertyGet);
                }
                return Ocx.Caption;
            }
            set
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("Caption", ActiveXInvokeKind.PropertySet);
                }
                Ocx.Caption = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(65547)]
        public virtual bool Titlebar
        {
            get
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("Titlebar", ActiveXInvokeKind.PropertyGet);
                }
                return Ocx.Titlebar;
            }
            set
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("Titlebar", ActiveXInvokeKind.PropertySet);
                }
                Ocx.Titlebar = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(65548)]
        public virtual bool Toolbars
        {
            get
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("Toolbars", ActiveXInvokeKind.PropertyGet);
                }
                return Ocx.Toolbars;
            }
            set
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("Toolbars", ActiveXInvokeKind.PropertySet);
                }
                Ocx.Toolbars = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(65549)]
        public virtual bool ModalState
        {
            get
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("ModalState", ActiveXInvokeKind.PropertyGet);
                }
                return Ocx.ModalState;
            }
            set
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("ModalState", ActiveXInvokeKind.PropertySet);
                }
                Ocx.ModalState = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(-504)]
        public virtual dsoBorderStyle BorderStyle
        {
            get
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("BorderStyle", ActiveXInvokeKind.PropertyGet);
                }
                return Ocx.BorderStyle;
            }
            set
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("BorderStyle", ActiveXInvokeKind.PropertySet);
                }
                Ocx.BorderStyle = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(-503)]
        [ComAliasName("System.UInt32")]
        public virtual Color BorderColor
        {
            get
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("BorderColor", ActiveXInvokeKind.PropertyGet);
                }
                return GetColorFromOleColor(((Ocx.BorderColor)));
            }
            set
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("BorderColor", ActiveXInvokeKind.PropertySet);
                }
                Ocx.BorderColor = ((GetOleColorFromColor(value)));
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(-501)]
        [ComAliasName("System.UInt32")]
        public override Color BackColor
        {
            get
            {
                if (((Ocx != null)
                     && PropsValid()))
                {
                    return GetColorFromOleColor(((Ocx.BackColor)));
                }
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                if ((Ocx != null))
                {
                    Ocx.BackColor = ((GetOleColorFromColor(value)));
                }
            }
        }

        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(-513)]
        [ComAliasName("System.UInt32")]
        public override Color ForeColor
        {
            get
            {
                if (((Ocx != null)
                     && PropsValid()))
                {
                    return GetColorFromOleColor(((Ocx.ForeColor)));
                }
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                if ((Ocx != null))
                {
                    Ocx.ForeColor = ((GetOleColorFromColor(value)));
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(65552)]
        [ComAliasName("System.UInt32")]
        public virtual Color TitlebarColor
        {
            get
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("TitlebarColor", ActiveXInvokeKind.PropertyGet);
                }
                return GetColorFromOleColor(((Ocx.TitlebarColor)));
            }
            set
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("TitlebarColor", ActiveXInvokeKind.PropertySet);
                }
                Ocx.TitlebarColor = ((GetOleColorFromColor(value)));
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [DispId(65553)]
        [ComAliasName("System.UInt32")]
        public virtual Color TitlebarTextColor
        {
            get
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("TitlebarTextColor", ActiveXInvokeKind.PropertyGet);
                }
                return GetColorFromOleColor(((Ocx.TitlebarTextColor)));
            }
            set
            {
                if ((Ocx == null))
                {
                    throw new InvalidActiveXStateException("TitlebarTextColor", ActiveXInvokeKind.PropertySet);
                }
                Ocx.TitlebarTextColor = ((GetOleColorFromColor(value)));
            }
        }

        public event DFramerCtlEventsOnFileCommandEventHandler OnFileCommand;

        public event DFramerCtlEventsOnDocumentOpenedEventHandler OnDocumentOpened;

        public event EventHandler OnDocumentClosed;

        public virtual void Activate()
        {
            if ((Ocx == null))
            {
                throw new InvalidActiveXStateException("Activate", ActiveXInvokeKind.MethodInvoke);
            }
            Ocx.Activate();
        }

        public virtual void CreateNew(string progId)
        {
            if ((Ocx == null))
            {
                throw new InvalidActiveXStateException("CreateNew", ActiveXInvokeKind.MethodInvoke);
            }
            Ocx.CreateNew(progId);
        }

        public virtual void Open(object document)
        {
            if ((Ocx == null))
            {
                throw new InvalidActiveXStateException("Open", ActiveXInvokeKind.MethodInvoke);
            }
            var paramArray = new[]
                                 {
                                     document,
                                     Missing.Value,
                                     Missing.Value,
                                     Missing.Value,
                                     Missing.Value
                                 };
            Type typeVar = typeof (_FramerControl);
            MethodInfo methodToInvoke = typeVar.GetMethod("Open");
            methodToInvoke.Invoke(Ocx, paramArray);
        }

        public virtual void Open(object document, object readOnly, object progId, object webUsername, object webPassword)
        {
            if ((Ocx == null))
            {
                throw new InvalidActiveXStateException("Open", ActiveXInvokeKind.MethodInvoke);
            }
            Ocx.Open(document, readOnly, progId, webUsername, webPassword);
        }

        public virtual void Save()
        {
            if ((Ocx == null))
            {
                throw new InvalidActiveXStateException("Save", ActiveXInvokeKind.MethodInvoke);
            }
            var paramArray = new object[]
                                 {
                                     Missing.Value,
                                     Missing.Value,
                                     Missing.Value,
                                     Missing.Value
                                 };
            Type typeVar = typeof (_FramerControl);
            MethodInfo methodToInvoke = typeVar.GetMethod("Save");
            methodToInvoke.Invoke(Ocx, paramArray);
        }

        public virtual void Save(object saveAsDocument, object overwriteExisting, object webUsername, object webPassword)
        {
            if ((Ocx == null))
            {
                throw new InvalidActiveXStateException("Save", ActiveXInvokeKind.MethodInvoke);
            }
            Ocx.Save(saveAsDocument, overwriteExisting, webUsername, webPassword);
        }

        public virtual void PrintOut()
        {
            if ((Ocx == null))
            {
                throw new InvalidActiveXStateException("PrintOut", ActiveXInvokeKind.MethodInvoke);
            }
            var paramArray = new object[]
                                 {
                                     Missing.Value
                                 };
            Type typeVar = typeof (_FramerControl);
            MethodInfo methodToInvoke = typeVar.GetMethod("PrintOut");
            methodToInvoke.Invoke(Ocx, paramArray);
        }

        public virtual void PrintOut(object promptToSelectPrinter)
        {
            if ((Ocx == null))
            {
                throw new InvalidActiveXStateException("PrintOut", ActiveXInvokeKind.MethodInvoke);
            }
            Ocx.PrintOut(promptToSelectPrinter);
        }

        public virtual void Close()
        {
            if ((Ocx == null))
            {
                throw new InvalidActiveXStateException("Close", ActiveXInvokeKind.MethodInvoke);
            }
            Ocx.Close();
        }

        public virtual void ShowDialog(dsoShowDialogType dlgType)
        {
            if ((Ocx == null))
            {
                throw new InvalidActiveXStateException("ShowDialog", ActiveXInvokeKind.MethodInvoke);
            }
            Ocx.ShowDialog(dlgType);
        }

        public virtual void SetEnableFileCommand(dsoFileCommandType item, bool pbool)
        {
            if ((Ocx == null))
            {
                throw new InvalidActiveXStateException("set_EnableFileCommand", ActiveXInvokeKind.MethodInvoke);
            }
            Ocx.set_EnableFileCommand(item, pbool);
        }

        public virtual bool GetEnableFileCommand(dsoFileCommandType item)
        {
            if ((Ocx == null))
            {
                throw new InvalidActiveXStateException("get_EnableFileCommand", ActiveXInvokeKind.MethodInvoke);
            }
            bool returnValue = ((Ocx.get_EnableFileCommand(item)));
            return returnValue;
        }

        protected override void CreateSink()
        {
            try
            {
                EventMulticaster = new AxFramerControlEventMulticaster(this);
                Cookie = new ConnectionPointCookie(Ocx, EventMulticaster, typeof (_DFramerCtlEvents));
            }
            catch (Exception)
            {
            }
        }

        protected override void DetachSink()
        {
            try
            {
                Cookie.Disconnect();
            }
            catch (Exception)
            {
            }
        }

        protected override void AttachInterfaces()
        {
            try
            {
                Ocx = ((_FramerControl) (GetOcx()));
            }
            catch (Exception)
            {
            }
        }

        internal void RaiseOnOnFileCommand(object sender, DFramerCtlEventsOnFileCommandEvent e)
        {
            if ((OnFileCommand != null))
            {
                OnFileCommand(sender, e);
            }
        }

        internal void RaiseOnOnDocumentOpened(object sender, DFramerCtlEventsOnDocumentOpenedEvent e)
        {
            if ((OnDocumentOpened != null))
            {
                OnDocumentOpened(sender, e);
            }
        }

        internal void RaiseOnOnDocumentClosed(object sender, EventArgs e)
        {
            if ((OnDocumentClosed != null))
            {
                OnDocumentClosed(sender, e);
            }
        }
    }

    public delegate void DFramerCtlEventsOnFileCommandEventHandler(
        object sender, DFramerCtlEventsOnFileCommandEvent e);

    [Guid("D3F6ADC0-BB82-49F3-8EFD-5BD620B56D1B")]
    public class DFramerCtlEventsOnFileCommandEvent
    {
        public readonly bool Cancel;
        private dsoFileCommandType Item;

        public DFramerCtlEventsOnFileCommandEvent(dsoFileCommandType item, bool cancel)
        {
            Item = item;
            Cancel = cancel;
        }
    }

    public delegate void DFramerCtlEventsOnDocumentOpenedEventHandler(
        object sender, DFramerCtlEventsOnDocumentOpenedEvent e);

    [Guid("D6591DF3-6B7D-42E0-9D68-AF71E9B19F67")]
    public class DFramerCtlEventsOnDocumentOpenedEvent
    {
        private object Document;
        private string ThisFile;

        public DFramerCtlEventsOnDocumentOpenedEvent(string thisFile, object document)
        {
            ThisFile = thisFile;
            Document = document;
        }
    }

    [ClassInterface(ClassInterfaceType.None)]
    [Guid("88656185-D238-4C98-B0C1-B9AE5142124E")]
    public sealed class AxFramerControlEventMulticaster : _DFramerCtlEvents
    {
        private readonly EmbeddedWordComponent Parent;

        public AxFramerControlEventMulticaster(EmbeddedWordComponent parent)
        {
            Parent = parent;
        }

        public void OnFileCommand(dsoFileCommandType item, ref bool cancel)
        {
            var onfilecommandEvent = new DFramerCtlEventsOnFileCommandEvent(item, cancel);
            Parent.RaiseOnOnFileCommand(Parent, onfilecommandEvent);
            cancel = onfilecommandEvent.Cancel;
        }

        public void OnDocumentOpened(string file, object document)
        {
            var ondocumentopenedEvent = new DFramerCtlEventsOnDocumentOpenedEvent(file, document);
            Parent.RaiseOnOnDocumentOpened(Parent, ondocumentopenedEvent);
        }

        public void OnDocumentClosed()
        {
            var ondocumentclosedEvent = new EventArgs();
            Parent.RaiseOnOnDocumentClosed(Parent, ondocumentclosedEvent);
        }
    }
}