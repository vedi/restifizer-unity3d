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
		public string Tag;
		
		public RestifizerResponse(int status, Hashtable result, string tag) {
			this.IsList = false;
			this.Status = status;
			this.HasError = false;
			this.Tag = tag;
		}

		public RestifizerResponse(int status, ArrayList result, string tag) {
			this.IsList = true;
			this.Status = status;
			this.HasError = false;
			this.Tag = tag;
		}

		public RestifizerResponse(int status, RestifizerError error, string tag) {
			this.Status = status;
			this.HasError = true;
			this.Error = error;
			this.Tag = tag;
		}
	}
}
