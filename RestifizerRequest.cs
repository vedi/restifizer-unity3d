using UnityEngine;
using System;
using System.Collections;

public class RestifizerRequest {
	enum AuthType {
		None,
		Client,
		Bearer
	};
	public string Path;
	public string Method;
	public bool FetchList = true;

    private Hashtable filterParams;

    private AuthType authType = AuthType.None;

	private RestifizerParams restifizerParams;

	public RestifizerRequest(RestifizerParams restifizerParams) {
		this.restifizerParams = restifizerParams;

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
	
	public RestifizerRequest Filter(String key, object value) {
        if (filterParams == null) {
            filterParams = new Hashtable();
        }
        filterParams[key] = value;
        
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
        RestifizerRequest restifizerRequest = new RestifizerRequest(restifizerParams);
        restifizerRequest.Path = Path;
        restifizerRequest.Method = Method;
        restifizerRequest.FetchList = FetchList;
        if (filterParams != null) {
            restifizerRequest.filterParams = filterParams.Clone() as Hashtable;
        }
        restifizerRequest.authType = authType;
        return restifizerRequest;
    }

    private void performRequest(string method, Hashtable parameters = null, Action<RestifizerResponse> callback = null) {

		HTTP.Request someRequest;

        string url = Path;
        string queryStr = "";

        if (filterParams != null && filterParams.Count > 0) {
            string filterValue = JSON.JsonEncode(filterParams);
            queryStr += "filter=" + filterValue;
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
			}
			else{
				someRequest = new HTTP.Request(method, url, parameters);
			}
			someRequest.SetHeader("Authorization", "Bearer " + restifizerParams.GetAccessToken());
		} else {
            someRequest = new HTTP.Request(method, url);
		}
		
		// Perform request
		someRequest.Send( ( request ) => {
			if (request.response == null) {
                Debug.LogWarning( "Server is not available"); // TODO: Get sure
				callback(null);
				return;
			}
			bool result = false;
            object responseResult = JSON.JsonDecode(request.response.Text, ref result);
            if (!result) {
                Debug.LogWarning( "Could not parse JSON response!" );
                callback(null);
                return;
            }

            if (responseResult is ArrayList) {
                callback(new RestifizerResponse(request.response.status, (ArrayList)responseResult));
            } else if (responseResult is Hashtable) {
                callback(new RestifizerResponse(request.response.status, (Hashtable)responseResult));
            } else {
                Debug.LogWarning("Unsupported type in response: " + responseResult.GetType());
                callback(null);
            }
		});
	}
}
