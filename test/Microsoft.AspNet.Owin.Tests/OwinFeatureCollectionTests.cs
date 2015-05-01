// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.FeatureModel;
using Microsoft.AspNet.Http;
using Xunit;

namespace Microsoft.AspNet.Owin
{
    public class OwinHttpEnvironmentTests
    {
        private T Get<T>(IFeatureCollection features)
        {
            object value;
            return features.TryGetValue(typeof(T), out value) ? (T)value : default(T);
        }
        private T Get<T>(IDictionary<string, object> env, string key)
        {
            object value;
            return env.TryGetValue(key, out value) ? (T)value : default(T);
        }

        [Fact]
        public void OwinHttpEnvironmentCanBeCreated()
        {
            var env = new Dictionary<string, object>
            {
                { "owin.RequestMethod", "POST" },
                { "owin.RequestPath", "/path" },
                { "owin.RequestPathBase", "/pathBase" },
                { "owin.RequestQueryString", "name=value" },
            };
            var features = new FeatureObject(new OwinFeatureCollection(env));

            var requestFeature = Get<IHttpRequestFeature>(features);
            Assert.Equal(requestFeature.Method, "POST");
            Assert.Equal(requestFeature.Path, "/path");
            Assert.Equal(requestFeature.PathBase, "/pathBase");
            Assert.Equal(requestFeature.QueryString, "?name=value");
        }

        [Fact]
        public void OwinHttpEnvironmentCanBeModified()
        {
            var env = new Dictionary<string, object>
            {
                { "owin.RequestMethod", "POST" },
                { "owin.RequestPath", "/path" },
                { "owin.RequestPathBase", "/pathBase" },
                { "owin.RequestQueryString", "name=value" },
            };
            var features = new FeatureObject(new OwinFeatureCollection(env));

            var requestFeature = Get<IHttpRequestFeature>(features);
            requestFeature.Method = "GET";
            requestFeature.Path = "/path2";
            requestFeature.PathBase = "/pathBase2";
            requestFeature.QueryString = "?name=value2";

            Assert.Equal("GET", Get<string>(env, "owin.RequestMethod"));
            Assert.Equal("/path2", Get<string>(env, "owin.RequestPath"));
            Assert.Equal("/pathBase2", Get<string>(env, "owin.RequestPathBase"));
            Assert.Equal("name=value2", Get<string>(env, "owin.RequestQueryString"));
        }

        [Fact]
        public void ImplementedInterfacesAreEnumerated()
        {
            var env = new Dictionary<string, object>
            {
                {"owin.RequestMethod", "POST"}
            };
            var features = new FeatureObject(new OwinFeatureCollection(env));

            var entries = features.ToArray();
            var keys = features.Keys.ToArray();
            var values = features.Values.ToArray();

            Assert.Contains(typeof(IHttpRequestFeature), keys);
            Assert.Contains(typeof(IHttpResponseFeature), keys);
        }
    }
}

