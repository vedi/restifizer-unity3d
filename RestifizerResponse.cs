using UnityEngine;
using System.Collections;
using HTTP;

namespace Restifizer {
	public class RestifizerResponse {
		public bool IsList;
		public bool HasError;
		public Hashtable Resource;
		public ArrayList ResourceList;
		public RestifizerError Error;
		public int Status;
		public string Tag;
		public HTTP.Request Request;
		
		public RestifizerResponse(HTTP.Request request, Hashtable result, string tag) {
			this.IsList = false;
			this.Status = request.response.status;
			this.Resource = result;
			this.HasError = false;
			this.Request = request;
            this.Tag = tag;
		}

		public RestifizerResponse(HTTP.Request request, ArrayList result, string tag) {
			this.IsList = true;
			this.Status = request.response.status;
			this.ResourceList = result;
            this.HasError = false;
			this.Request = request;
			this.Tag = tag;
		}

		public RestifizerResponse(HTTP.Request request, RestifizerError error, string tag) {
			this.Status = request.response.status;
			this.HasError = true;
			this.Error = error;
			this.Request = request;
			this.Tag = tag;
		}
	}
}
