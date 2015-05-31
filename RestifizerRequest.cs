using UnityEngine;
using System;
using System.Collections;

namespace Restifizer {
	public class RestifizerRequest {
		enum AuthType {
			None,
			Client,
			Bearer
		};
		public string Path;
		public string Method;
		public bool FetchList = true;
		public string Tag;
		public int PageNumber = -1;
		public int PageSize = -1;


		private Hashtable filterParams;

		private Hashtable extraQuery;

		private AuthType authType = AuthType.None;
		
		private RestifizerParams restifizerParams;
		private IErrorHandler errorHandler;
		
		public RestifizerRequest(RestifizerParams restifizerParams, IErrorHandler errorHandler) {
			this.restifizerParams = restifizerParams;
			this.errorHandler = errorHandler;
			
			this.Path = "";
		}
		
		public RestifizerRequest WithClientAuth() {
			this.authType = AuthType.Client;
			
			return this;
		}
		
		public RestifizerRequest WithBearerAuth() {
			this.authType = AuthType.Bearer;
			
			return this;
		}

		public RestifizerRequest WithTag(string tag) {
			this.Tag = tag;
			
			return this;
		}

		public RestifizerRequest Filter(String key, object value) {
			if (filterParams == null) {
				filterParams = new Hashtable();
			}
			filterParams[key] = value;
			
			return this;
		}

		public RestifizerRequest Query(String key, object value) {
			if (extraQuery == null) {
				extraQuery = new Hashtable();
			}
			extraQuery[key] = value;
			
			return this;
		}
		
		public RestifizerRequest Page(int pageNumber, int pageSize) {
			this.PageNumber = pageNumber;
			this.PageSize = pageSize;
			
			return this;
		}
		
		public RestifizerRequest Page(int pageNumber) {
			this.PageNumber = pageNumber;

			return this;
		}

		public RestifizerRequest SetPageSize(int pageSize) {
			this.PageSize = pageSize;
			
			return this;
		}
		
		public RestifizerRequest One(String id) {
			this.Path += "/" + id;
			this.FetchList = false;
			
			return this;
		}
		
		public void List(String path) {
			this.Path += "/" + path;
			this.FetchList = true;
		}
		
		public void Get(Action<RestifizerResponse> callback = null) {
			performRequest("get", null, callback);
		}
		
		public void Post(Hashtable parameters = null, Action<RestifizerResponse> callback = null) {
			performRequest("post", parameters, callback);
		}
		
		public void Put(Hashtable parameters = null, Action<RestifizerResponse> callback = null) {
			performRequest("put", parameters, callback);
		}
		
		public void Patch(Hashtable parameters = null, Action<RestifizerResponse> callback = null) {
			performRequest("patch", parameters, callback);
		}
		
		public RestifizerRequest Copy() {
			RestifizerRequest restifizerRequest = new RestifizerRequest(restifizerParams, errorHandler);
			restifizerRequest.Path = Path;
			restifizerRequest.Method = Method;
			restifizerRequest.Tag = Tag;
			restifizerRequest.PageNumber = PageNumber;
			restifizerRequest.PageSize = PageSize;
			restifizerRequest.FetchList = FetchList;
			if (filterParams != null) {
				restifizerRequest.filterParams = filterParams.Clone() as Hashtable;
			}
			if (extraQuery != null) {
				restifizerRequest.extraQuery = extraQuery.Clone() as Hashtable;
			}
			restifizerRequest.authType = authType;
			return restifizerRequest;
		}
		
		private void performRequest(string method, Hashtable parameters = null, Action<RestifizerResponse> callback = null) {
			
			HTTP.Request someRequest;
			
			string url = Path;
			string queryStr = "";

			// paging
			if (PageNumber != -1) {
				if (queryStr.Length > 0) {
					queryStr += "&";
				}
				queryStr += "per_page=" + PageNumber;
			}
			if (PageSize != -1) {
				if (queryStr.Length > 0) {
					queryStr += "&";
				}
				queryStr += "page=" + PageSize;
			}

			// filtering
			if (filterParams != null && filterParams.Count > 0) {
				if (queryStr.Length > 0) {
					queryStr += "&";
				}
				string filterValue = JSON.JsonEncode(filterParams);
				queryStr += "filter=" + filterValue;
			}

			// extra params
			if (extraQuery != null && extraQuery.Count > 0) {
				foreach (string key in extraQuery.Keys) {
					if (queryStr.Length > 0) {
						queryStr += "&";
					}
					queryStr += key + "=" + extraQuery[key];
				}
			}

			if (queryStr.Length > 0) {
				url += "?" + queryStr;
			}
			
			
			// Handle authentication
			if (this.authType == AuthType.Client) {
				if (parameters == null) {
					parameters = new Hashtable();
				}
				parameters.Add( "client_id", restifizerParams.GetClientId() );
				parameters.Add( "client_secret", restifizerParams.GetClientSecret() );
				
				someRequest = new HTTP.Request(method, url, parameters);
			} else if (this.authType == AuthType.Bearer) {
				if (parameters == null) {
					someRequest = new HTTP.Request(method, url);
				} else {
					someRequest = new HTTP.Request(method, url, parameters);
				}
				someRequest.SetHeader("Authorization", "Bearer " + restifizerParams.GetAccessToken());
			} else {
				if (parameters == null) {
					someRequest = new HTTP.Request(method, url);
				} else {
					someRequest = new HTTP.Request(method, url, parameters);
				}
			}

			string tag = this.Tag;
			// Perform request
			someRequest.Send( ( request ) => {
				if (request.response == null) {
					RestifizerError error = RestifizerErrorFactory.Create(-1, null, tag);
					if (errorHandler != null) {
						bool propagateResult = !errorHandler.onRestifizerError(error);
						if (propagateResult) {
							callback(null);
						}
					} else {
						callback(null);
					}
					return;
				}
				bool result = false;
				object responseResult = JSON.JsonDecode(request.response.Text, ref result);
				if (!result) {
					RestifizerError error = RestifizerErrorFactory.Create(-2, request.response.Text, tag);
					if (errorHandler != null) {
						bool propagateResult = !errorHandler.onRestifizerError(error);
						if (propagateResult) {
							callback(null);
						}
					} else {
						callback(null);
					}
					return;
				}

				bool hasError = request.response.status >= 300;
				if (hasError) {
					RestifizerError error = RestifizerErrorFactory.Create(request.response.status, responseResult, tag);
					if (errorHandler != null) {
						bool propagateResult = !errorHandler.onRestifizerError(error);
						if (propagateResult) {
							callback(new RestifizerResponse(request, error, tag));
						}
					} else {
						callback(new RestifizerResponse(request, error, tag));
					}
				} else if (responseResult is ArrayList) {
					callback(new RestifizerResponse(request, (ArrayList)responseResult, tag));
				} else if (responseResult is Hashtable) {
					callback(new RestifizerResponse(request, (Hashtable)responseResult, tag));
				} else {
					Debug.LogWarning("Unsupported type in response: " + responseResult.GetType());
					callback(null);
				}
			});
		}
	}
}
