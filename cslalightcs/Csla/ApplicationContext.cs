﻿using System;
using System.Threading;
using System.Security.Principal;
using System.Collections.Generic;
using System.Configuration;
using Csla.Core;

namespace Csla
{
  /// <summary>
  /// Provides consistent context information between the client
  /// and server DataPortal objects. 
  /// </summary>
  public static class ApplicationContext
  {
    #region User

    private static IPrincipal _principal;

    /// <summary>
    /// Get or set the current <see cref="IPrincipal" />
    /// object representing the user's identity.
    /// </summary>
    public static IPrincipal User
    {
      get
      {
        return _principal;
      }
      set
      {
        _principal = value;
      }
    }

    #endregion

    #region LocalContext

    private static object _syncLocalContext = new object();
    public static object LocalContextSync
    {
      get { return _syncLocalContext; }
    }

    private const string _localContextName = "Csla.LocalContext";

    private static ContextDictionary _localContext;
    /// <summary>
    /// Returns the application-specific context data that
    /// is local to the current AppDomain.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The return value is a Dictionary<string, object>. If one does
    /// not already exist, and empty one is created and returned.
    /// </para><para>
    /// Note that data in this context is NOT transferred to and from
    /// the client and server.
    /// </para>
    /// </remarks>
    public static ContextDictionary LocalContext
    {
      get
      {
        if (_localContext == null)
          lock (_syncLocalContext)
            if(_localContext == null)
              _localContext = new ContextDictionary();

        return _localContext;
      }
    }

    #endregion

    #region Client/Global Context
    
    private static object _syncClientContext = new object();
    public static object ClientContextSync
    {
      get { return _syncClientContext; }
    }

    private static object _syncGlobalContext = new object();
    public static object GlobalContextSync
    {
      get { return _syncGlobalContext; }
    }

    private const string _clientContextName = "Csla.ClientContext";
    private const string _globalContextName = "Csla.GlobalContext";

    private static ContextDictionary _clientContext;
    /// <summary>
    /// Returns the application-specific context data provided
    /// by the client.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The return value is a Dictionary<string, object>. If one does
    /// not already exist, and empty one is created and returned.
    /// </para><para>
    /// Note that data in this context is transferred from
    /// the client to the server. No data is transferred from
    /// the server to the client.
    /// </para><para>
    /// This property is thread safe in a Windows client
    /// setting and on an application server. It is not guaranteed
    /// to be thread safe within the context of an ASP.NET
    /// client setting (i.e. in your ASP.NET UI).
    /// </para>
    /// </remarks>
    public static ContextDictionary ClientContext
    {
      get
      {
        if (_clientContext == null)
          lock (_syncClientContext)
            if(_clientContext == null)
              _clientContext = new ContextDictionary();

        return _clientContext;
      }
    }

    private static ContextDictionary _globalContext;
    /// <summary>
    /// Returns the application-specific context data shared
    /// on both client and server.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The return value is a Dictionary<string, object>. If one does
    /// not already exist, and empty one is created and returned.
    /// </para><para>
    /// Note that data in this context is transferred to and from
    /// the client and server. Any objects or data in this context
    /// will be transferred bi-directionally across the network.
    /// </para>
    /// </remarks>
    public static ContextDictionary GlobalContext
    {
      get
      {
        if (_globalContext == null)
          lock (_syncGlobalContext)
            if(_globalContext == null)
              _globalContext = new ContextDictionary();

        return _globalContext;
      }
    }

    internal static void SetContext(
      ContextDictionary clientContext,
      ContextDictionary globalContext)
    {
      lock (_syncClientContext)
        _clientContext = clientContext;
      lock (_syncGlobalContext)
        _globalContext = globalContext;
    }

    /// <summary>
    /// Clears all context collections.
    /// </summary>
    public static void Clear()
    {
      SetContext(null, null);
      lock (_syncLocalContext)
        _localContext = null;
    }

    #endregion

    #region Config Settings

    /// <summary>
    /// Returns the authentication type being used by the
    /// CSLA .NET framework.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// This value is read from the application configuration
    /// file with the key value "CslaAuthentication". The value
    /// "Windows" indicates CSLA .NET should use Windows integrated
    /// (or AD) security. Any other value indicates the use of
    /// custom security derived from BusinessPrincipalBase.
    /// </remarks>
    public static string AuthenticationType
    {
      get { return "Csla"; }
    }

    /// <summary>
    /// Returns the channel or network protocol
    /// for the DataPortal server.
    /// </summary>
    /// <value>Fully qualified assembly/type name of the proxy class.</value>
    /// <returns></returns>
    /// <remarks>
    /// <para>
    /// This value is read from the application configuration
    /// file with the key value "CslaDataPortalProxy". 
    /// </para><para>
    /// The proxy class must implement Csla.Server.IDataPortalServer.
    /// </para><para>
    /// The value "Local" is a shortcut to running the DataPortal
    /// "server" in the client process.
    /// </para><para>
    /// Other built-in values include:
    /// <list>
    /// <item>
    /// <term>Csla,Csla.DataPortalClient.RemotingProxy</term>
    /// <description>Use .NET Remoting to communicate with the server</description>
    /// </item>
    /// <item>
    /// <term>Csla,Csla.DataPortalClient.EnterpriseServicesProxy</term>
    /// <description>Use Enterprise Services (DCOM) to communicate with the server</description>
    /// </item>
    /// <item>
    /// <term>Csla,Csla.DataPortalClient.WebServicesProxy</term>
    /// <description>Use Web Services (asmx) to communicate with the server</description>
    /// </item>
    /// </list>
    /// Each proxy type does require that the DataPortal server be hosted using the appropriate
    /// technology. For instance, Web Services and Remoting should be hosted in IIS, while
    /// Enterprise Services must be hosted in COM+.
    /// </para>
    /// </remarks>
    public static string DataPortalProxy
    {
      get
      {
        string result = null; // = ConfigurationManager.AppSettings["CslaDataPortalProxy"];
        if (string.IsNullOrEmpty(result))
          result = "Local";
        return result;
      }
    }

    private static PropertyChangedModes _propertyChangedMode = PropertyChangedModes.Xaml;
    /// <summary>
    /// Gets or sets a value specifying how CSLA .NET should
    /// raise PropertyChanged events.
    /// </summary>
    public static PropertyChangedModes PropertyChangedMode
    {
      get
      {
        return _propertyChangedMode;
      }
      set
      {
        _propertyChangedMode = value;
      }
    }

    /// <summary>
    /// Enum representing the locations code can execute.
    /// </summary>
    public enum ExecutionLocations
    {
      /// <summary>
      /// The code is executing on the client.
      /// </summary>
      Client,
      /// <summary>
      /// The code is executing on the application server.
      /// </summary>
      Server
    }

    /// <summary>
    /// Enum representing the way in which CSLA .NET
    /// should raise PropertyChanged events.
    /// </summary>
    public enum PropertyChangedModes
    {
      /// <summary>
      /// Raise PropertyChanged events as required
      /// by Windows Forms data binding.
      /// </summary>
      Windows,
      /// <summary>
      /// Raise PropertyChanged events as required
      /// by XAML data binding in WPF.
      /// </summary>
      Xaml
    }

    #endregion

    #region In-Memory Settings

    private static ExecutionLocations _executionLocation =
      ExecutionLocations.Client;

    /// <summary>
    /// Returns a value indicating whether the application code
    /// is currently executing on the client or server.
    /// </summary>
    public static ExecutionLocations ExecutionLocation
    {
      get { return _executionLocation; }
    }

    internal static void SetExecutionLocation(ExecutionLocations location)
    {
      _executionLocation = location;
    }

    #endregion

  }
}