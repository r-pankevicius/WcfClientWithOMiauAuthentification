﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LazyCatConsole.LazyCatServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="LazyCatServiceReference.ILazyCatService")]
    public interface ILazyCatService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILazyCatService/SumWithAnonymousAuth", ReplyAction="http://tempuri.org/ILazyCatService/SumWithAnonymousAuthResponse")]
        int SumWithAnonymousAuth(int a, int b);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILazyCatService/SumWithAnonymousAuth", ReplyAction="http://tempuri.org/ILazyCatService/SumWithAnonymousAuthResponse")]
        System.Threading.Tasks.Task<int> SumWithAnonymousAuthAsync(int a, int b);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILazyCatService/SumWithOMiauAuth", ReplyAction="http://tempuri.org/ILazyCatService/SumWithOMiauAuthResponse")]
        int SumWithOMiauAuth(int a, int b);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILazyCatService/SumWithOMiauAuth", ReplyAction="http://tempuri.org/ILazyCatService/SumWithOMiauAuthResponse")]
        System.Threading.Tasks.Task<int> SumWithOMiauAuthAsync(int a, int b);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILazyCatService/GetOMiauToken_WithClientCredentials", ReplyAction="http://tempuri.org/ILazyCatService/GetOMiauToken_WithClientCredentialsResponse")]
        string GetOMiauToken_WithClientCredentials(string cat_id, string cat_secret);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ILazyCatService/GetOMiauToken_WithClientCredentials", ReplyAction="http://tempuri.org/ILazyCatService/GetOMiauToken_WithClientCredentialsResponse")]
        System.Threading.Tasks.Task<string> GetOMiauToken_WithClientCredentialsAsync(string cat_id, string cat_secret);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ILazyCatServiceChannel : LazyCatConsole.LazyCatServiceReference.ILazyCatService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class LazyCatServiceClient : System.ServiceModel.ClientBase<LazyCatConsole.LazyCatServiceReference.ILazyCatService>, LazyCatConsole.LazyCatServiceReference.ILazyCatService {
        
        public LazyCatServiceClient() {
        }
        
        public LazyCatServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public LazyCatServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LazyCatServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public LazyCatServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public int SumWithAnonymousAuth(int a, int b) {
            return base.Channel.SumWithAnonymousAuth(a, b);
        }
        
        public System.Threading.Tasks.Task<int> SumWithAnonymousAuthAsync(int a, int b) {
            return base.Channel.SumWithAnonymousAuthAsync(a, b);
        }
        
        public int SumWithOMiauAuth(int a, int b) {
            return base.Channel.SumWithOMiauAuth(a, b);
        }
        
        public System.Threading.Tasks.Task<int> SumWithOMiauAuthAsync(int a, int b) {
            return base.Channel.SumWithOMiauAuthAsync(a, b);
        }
        
        public string GetOMiauToken_WithClientCredentials(string cat_id, string cat_secret) {
            return base.Channel.GetOMiauToken_WithClientCredentials(cat_id, cat_secret);
        }
        
        public System.Threading.Tasks.Task<string> GetOMiauToken_WithClientCredentialsAsync(string cat_id, string cat_secret) {
            return base.Channel.GetOMiauToken_WithClientCredentialsAsync(cat_id, cat_secret);
        }
    }
}