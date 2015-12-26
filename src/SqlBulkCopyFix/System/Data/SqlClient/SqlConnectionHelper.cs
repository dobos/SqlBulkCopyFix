// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// In the desktop version of the framework, this file is generated from ProviderBase\DbConnectionHelper.cs
// #line 1 "e:\\fxdata\\src\\ndp\\fx\\src\\data\\system\\data\\providerbase\\dbconnectionhelper.cs"

using System.Data.Common;
using System.Data.Common.Custom;
using System.Data.ProviderBase;
using System.Data.ProviderBase.Custom;
using System.Diagnostics;
using System.Threading;


namespace System.Data.SqlClient.Custom
{
    public sealed partial class SqlConnection : DbConnection
    {
        private static readonly DbConnectionFactory s_connectionFactory = SqlConnectionFactory.SingletonInstance;

        private DbConnectionOptions _userConnectionOptions;
        private DbConnectionPoolGroup _poolGroup;
        private DbConnectionInternal _innerConnection;
        private int _closeCount;


        public SqlConnection() : base()
        {
            GC.SuppressFinalize(this);
            _innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
        }

        internal int CloseCount
        {
            get
            {
                return _closeCount;
            }
        }

        internal DbConnectionFactory ConnectionFactory
        {
            get
            {
                return s_connectionFactory;
            }
        }

        internal DbConnectionOptions ConnectionOptions
        {
            get
            {
                System.Data.ProviderBase.Custom.DbConnectionPoolGroup poolGroup = PoolGroup;
                return ((null != poolGroup) ? poolGroup.ConnectionOptions : null);
            }
        }

        private string ConnectionString_Get()
        {
            bool hidePassword = InnerConnection.ShouldHidePassword;
            DbConnectionOptions connectionOptions = UserConnectionOptions;
            return ((null != connectionOptions) ? connectionOptions.UsersConnectionString(hidePassword) : "");
        }

        private void ConnectionString_Set(DbConnectionPoolKey key)
        {
            DbConnectionOptions connectionOptions = null;
            System.Data.ProviderBase.Custom.DbConnectionPoolGroup poolGroup = ConnectionFactory.GetConnectionPoolGroup(key, null, ref connectionOptions);
            DbConnectionInternal connectionInternal = InnerConnection;
            bool flag = connectionInternal.AllowSetConnectionString;
            if (flag)
            {
                flag = SetInnerConnectionFrom(DbConnectionClosedBusy.SingletonInstance, connectionInternal);
                if (flag)
                {
                    _userConnectionOptions = connectionOptions;
                    _poolGroup = poolGroup;
                    _innerConnection = DbConnectionClosedNeverOpened.SingletonInstance;
                }
            }
            if (!flag)
            {
                throw ADP.OpenConnectionPropertySet(ADP.ConnectionString, connectionInternal.State);
            }
        }

        internal DbConnectionInternal InnerConnection
        {
            get
            {
                return _innerConnection;
            }
        }

        internal System.Data.ProviderBase.Custom.DbConnectionPoolGroup PoolGroup
        {
            get
            {
                return _poolGroup;
            }
            set
            {
                Debug.Assert(null != value, "null poolGroup");
                _poolGroup = value;
            }
        }


        internal DbConnectionOptions UserConnectionOptions
        {
            get
            {
                return _userConnectionOptions;
            }
        }


        internal void Abort(Exception e)
        {
            DbConnectionInternal innerConnection = _innerConnection;
            if (ConnectionState.Open == innerConnection.State)
            {
                Interlocked.CompareExchange(ref _innerConnection, DbConnectionClosedPreviouslyOpened.SingletonInstance, innerConnection);
                innerConnection.DoomThisConnection();
            }
        }

        internal void AddWeakReference(object value, int tag)
        {
            InnerConnection.AddWeakReference(value, tag);
        }

        override protected DbCommand CreateDbCommand()
        {
            DbCommand command = null;
            DbProviderFactory providerFactory = ConnectionFactory.ProviderFactory;
            command = providerFactory.CreateCommand();
            command.Connection = this;
            return command;
        }


        override protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                _userConnectionOptions = null;
                _poolGroup = null;
                Close();
            }
            DisposeMe(disposing);
            base.Dispose(disposing);
        }

        partial void RepairInnerConnection();


        internal void NotifyWeakReference(int message)
        {
            InnerConnection.NotifyWeakReference(message);
        }

        internal void PermissionDemand()
        {
            Debug.Assert(DbConnectionClosedConnecting.SingletonInstance == _innerConnection, "not connecting");
            System.Data.ProviderBase.Custom.DbConnectionPoolGroup poolGroup = PoolGroup;
            DbConnectionOptions connectionOptions = ((null != poolGroup) ? poolGroup.ConnectionOptions : null);
            if ((null == connectionOptions) || connectionOptions.IsEmpty)
            {
                throw ADP.NoConnectionString();
            }
            DbConnectionOptions userConnectionOptions = UserConnectionOptions;
            Debug.Assert(null != userConnectionOptions, "null UserConnectionOptions");
        }

        internal void RemoveWeakReference(object value)
        {
            InnerConnection.RemoveWeakReference(value);
        }

        internal void SetInnerConnectionEvent(DbConnectionInternal to)
        {
            Debug.Assert(null != _innerConnection, "null InnerConnection");
            Debug.Assert(null != to, "to null InnerConnection");

            ConnectionState originalState = _innerConnection.State & ConnectionState.Open;
            ConnectionState currentState = to.State & ConnectionState.Open;
            if ((originalState != currentState) && (ConnectionState.Closed == currentState))
            {
                unchecked { _closeCount++; }
            }

            _innerConnection = to;
            if (ConnectionState.Closed == originalState && ConnectionState.Open == currentState)
            {
                OnStateChange(DbConnectionInternal.StateChangeOpen);
            }
            else if (ConnectionState.Open == originalState && ConnectionState.Closed == currentState)
            {
                OnStateChange(DbConnectionInternal.StateChangeClosed);
            }
            else
            {
                Debug.Assert(false, "unexpected state switch");
                if (originalState != currentState)
                {
                    OnStateChange(new StateChangeEventArgs(originalState, currentState));
                }
            }
        }

        internal bool SetInnerConnectionFrom(DbConnectionInternal to, DbConnectionInternal from)
        {
            Debug.Assert(null != _innerConnection, "null InnerConnection");
            Debug.Assert(null != from, "from null InnerConnection");
            Debug.Assert(null != to, "to null InnerConnection");
            bool result = (from == Interlocked.CompareExchange<DbConnectionInternal>(ref _innerConnection, to, from));
            return result;
        }

        internal void SetInnerConnectionTo(DbConnectionInternal to)
        {
            Debug.Assert(null != _innerConnection, "null InnerConnection");
            Debug.Assert(null != to, "to null InnerConnection");
            _innerConnection = to;
        }
    }
}

