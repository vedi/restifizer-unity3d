using UnityEngine;
using System.Collections;

namespace Restifizer {
	public class RestifizerError {
		public int Status;
		public Hashtable ErrorRaw;
		public ArrayList ErrorListRaw;
		
		public RestifizerError(int status, Hashtable error) {
			this.Status = status;
			this.ErrorRaw = error;
		}
		
		public RestifizerError(int status, ArrayList error) {
			this.Status = status;
			this.ErrorListRaw = error;
		}
		
		override public string ToString() {
			string result = "status: " + Status + ", raw: ";
			if (ErrorRaw != null) {
				result += ErrorRaw.ToString();
			} else if (ErrorListRaw != null) {
				result += ErrorListRaw.ToString();
			} else {
				result += "<EMPTY>";
			}
			
			return result;
		}
	}
}
