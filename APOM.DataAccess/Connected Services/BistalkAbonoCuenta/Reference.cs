﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace APOM.DataAccess.BistalkAbonoCuenta {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="BistalkAbonoCuenta.BCPAbonosService")]
    public interface BCPAbonosService {
        
        // CODEGEN: Generating message contract since the operation BCPAbonosService is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="BCPAbonosService", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        APOM.DataAccess.BistalkAbonoCuenta.BCPAbonosServiceResponse BCPAbonosService(APOM.DataAccess.BistalkAbonoCuenta.BCPAbonosServiceRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1064.2")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://BCP.Biztalk.AbonoCuentaCorriente.Schemas.ServiceContract")]
    public partial class Request : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string nombreCanalField;
        
        private string passwordField;
        
        private string nroCuentaField;
        
        private string tipoDeCuentaField;
        
        private string monedaField;
        
        private string montoField;
        
        private string glosaField;
        
        private string tipodeCambioCompraField;
        
        private string tipodeCambioVentaField;
        
        private string fechaSolicitudCanalField;
        
        private string horaSolicitudCanalField;
        
        private string nroReferenciaCanalField;
        
        private string nroOperacionOriginalField;
        
        private string tetiField;
        
        private string userField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string NombreCanal {
            get {
                return this.nombreCanalField;
            }
            set {
                this.nombreCanalField = value;
                this.RaisePropertyChanged("NombreCanal");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string Password {
            get {
                return this.passwordField;
            }
            set {
                this.passwordField = value;
                this.RaisePropertyChanged("Password");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string NroCuenta {
            get {
                return this.nroCuentaField;
            }
            set {
                this.nroCuentaField = value;
                this.RaisePropertyChanged("NroCuenta");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string TipoDeCuenta {
            get {
                return this.tipoDeCuentaField;
            }
            set {
                this.tipoDeCuentaField = value;
                this.RaisePropertyChanged("TipoDeCuenta");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string Moneda {
            get {
                return this.monedaField;
            }
            set {
                this.monedaField = value;
                this.RaisePropertyChanged("Moneda");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string Monto {
            get {
                return this.montoField;
            }
            set {
                this.montoField = value;
                this.RaisePropertyChanged("Monto");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public string Glosa {
            get {
                return this.glosaField;
            }
            set {
                this.glosaField = value;
                this.RaisePropertyChanged("Glosa");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public string TipodeCambioCompra {
            get {
                return this.tipodeCambioCompraField;
            }
            set {
                this.tipodeCambioCompraField = value;
                this.RaisePropertyChanged("TipodeCambioCompra");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=8)]
        public string TipodeCambioVenta {
            get {
                return this.tipodeCambioVentaField;
            }
            set {
                this.tipodeCambioVentaField = value;
                this.RaisePropertyChanged("TipodeCambioVenta");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=9)]
        public string FechaSolicitudCanal {
            get {
                return this.fechaSolicitudCanalField;
            }
            set {
                this.fechaSolicitudCanalField = value;
                this.RaisePropertyChanged("FechaSolicitudCanal");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=10)]
        public string HoraSolicitudCanal {
            get {
                return this.horaSolicitudCanalField;
            }
            set {
                this.horaSolicitudCanalField = value;
                this.RaisePropertyChanged("HoraSolicitudCanal");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=11)]
        public string NroReferenciaCanal {
            get {
                return this.nroReferenciaCanalField;
            }
            set {
                this.nroReferenciaCanalField = value;
                this.RaisePropertyChanged("NroReferenciaCanal");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=12)]
        public string NroOperacionOriginal {
            get {
                return this.nroOperacionOriginalField;
            }
            set {
                this.nroOperacionOriginalField = value;
                this.RaisePropertyChanged("NroOperacionOriginal");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=13)]
        public string Teti {
            get {
                return this.tetiField;
            }
            set {
                this.tetiField = value;
                this.RaisePropertyChanged("Teti");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=14)]
        public string User {
            get {
                return this.userField;
            }
            set {
                this.userField = value;
                this.RaisePropertyChanged("User");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1064.2")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://BCP.Biztalk.AbonoCuentaCorriente.Schemas.ServiceContract")]
    public partial class Response : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string codRsptaField;
        
        private string msjRetornoField;
        
        private string nroOperacionHostField;
        
        private string tetiField;
        
        private string userField;
        
        private string traceBiztalkField;
        
        private string saldoContableField;
        
        private string saldoDisponibleField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=0)]
        public string CodRspta {
            get {
                return this.codRsptaField;
            }
            set {
                this.codRsptaField = value;
                this.RaisePropertyChanged("CodRspta");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=1)]
        public string MsjRetorno {
            get {
                return this.msjRetornoField;
            }
            set {
                this.msjRetornoField = value;
                this.RaisePropertyChanged("MsjRetorno");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=2)]
        public string NroOperacionHost {
            get {
                return this.nroOperacionHostField;
            }
            set {
                this.nroOperacionHostField = value;
                this.RaisePropertyChanged("NroOperacionHost");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=3)]
        public string Teti {
            get {
                return this.tetiField;
            }
            set {
                this.tetiField = value;
                this.RaisePropertyChanged("Teti");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=4)]
        public string User {
            get {
                return this.userField;
            }
            set {
                this.userField = value;
                this.RaisePropertyChanged("User");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=5)]
        public string TraceBiztalk {
            get {
                return this.traceBiztalkField;
            }
            set {
                this.traceBiztalkField = value;
                this.RaisePropertyChanged("TraceBiztalk");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=6)]
        public string SaldoContable {
            get {
                return this.saldoContableField;
            }
            set {
                this.saldoContableField = value;
                this.RaisePropertyChanged("SaldoContable");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form=System.Xml.Schema.XmlSchemaForm.Unqualified, Order=7)]
        public string SaldoDisponible {
            get {
                return this.saldoDisponibleField;
            }
            set {
                this.saldoDisponibleField = value;
                this.RaisePropertyChanged("SaldoDisponible");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class BCPAbonosServiceRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://BCP.Biztalk.AbonoCuentaCorriente.Schemas.ServiceContract", Order=0)]
        public APOM.DataAccess.BistalkAbonoCuenta.Request Request;
        
        public BCPAbonosServiceRequest() {
        }
        
        public BCPAbonosServiceRequest(APOM.DataAccess.BistalkAbonoCuenta.Request Request) {
            this.Request = Request;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class BCPAbonosServiceResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://BCP.Biztalk.AbonoCuentaCorriente.Schemas.ServiceContract", Order=0)]
        public APOM.DataAccess.BistalkAbonoCuenta.Response Response;
        
        public BCPAbonosServiceResponse() {
        }
        
        public BCPAbonosServiceResponse(APOM.DataAccess.BistalkAbonoCuenta.Response Response) {
            this.Response = Response;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface BCPAbonosServiceChannel : APOM.DataAccess.BistalkAbonoCuenta.BCPAbonosService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BCPAbonosServiceClient : System.ServiceModel.ClientBase<APOM.DataAccess.BistalkAbonoCuenta.BCPAbonosService>, APOM.DataAccess.BistalkAbonoCuenta.BCPAbonosService {
        
        public BCPAbonosServiceClient() {
        }
        
        public BCPAbonosServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public BCPAbonosServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BCPAbonosServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BCPAbonosServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        APOM.DataAccess.BistalkAbonoCuenta.BCPAbonosServiceResponse APOM.DataAccess.BistalkAbonoCuenta.BCPAbonosService.BCPAbonosService(APOM.DataAccess.BistalkAbonoCuenta.BCPAbonosServiceRequest request) {
            return base.Channel.BCPAbonosService(request);
        }
        
        public APOM.DataAccess.BistalkAbonoCuenta.Response BCPAbonosService(APOM.DataAccess.BistalkAbonoCuenta.Request Request) {
            APOM.DataAccess.BistalkAbonoCuenta.BCPAbonosServiceRequest inValue = new APOM.DataAccess.BistalkAbonoCuenta.BCPAbonosServiceRequest();
            inValue.Request = Request;
            APOM.DataAccess.BistalkAbonoCuenta.BCPAbonosServiceResponse retVal = ((APOM.DataAccess.BistalkAbonoCuenta.BCPAbonosService)(this)).BCPAbonosService(inValue);
            return retVal.Response;
        }
    }
}
