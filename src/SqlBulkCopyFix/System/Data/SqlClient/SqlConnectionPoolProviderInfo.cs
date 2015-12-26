// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


//------------------------------------------------------------------------------

using System.Data.ProviderBase.Custom;

namespace System.Data.SqlClient.Custom
{
    internal sealed class SqlConnectionPoolProviderInfo : DbConnectionPoolProviderInfo
    {
        private string _instanceName;

        internal string InstanceName
        {
            get
            {
                return _instanceName;
            }
            set
            {
                _instanceName = value;
            }
        }
    }
}
