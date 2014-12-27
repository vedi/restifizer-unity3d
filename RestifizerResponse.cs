using UnityEngine;
using System.Collections;

namespace Restifizer {
	public class RestifizerResponse {
		public bool IsList;
		public bool HasError;
		public Hashtable Resource;
		public ArrayList ResourceList;
		public RestifizerError Error;
		public int Status;
		
		public RestifizerResponse(int status, Hashtable result) {
			this.IsList = false;
			this.Status = status;
			this.HasError = status >= 300;
			if (this.HasError) {
				this.Error = new RestifizerError(status, result);
			} else {
				this.Resource = result;
			}
		}
		
		public RestifizerResponse(int status, ArrayList result) {
			this.IsList = true;
			this.Status = status;
			this.HasError = status >= 300;
			if (this.HasError) {
				this.Error = new RestifizerError(status, result);
			} else {
				this.ResourceList = result;
			}
		}
	}
}
