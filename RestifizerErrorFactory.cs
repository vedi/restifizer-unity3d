using UnityEngine;
using System.Collections;

namespace Restifizer {
	public class RestifizerErrorFactory {
		public static RestifizerError Create(int status, object error, string tag) {
			switch (status) {
			case -1: 
				return new ServerNotAvailableError(tag);
			case -2: 
				return new WrongResponseFormatError(error, tag);
			case 400: 
				return new BadRequestError(status, error, tag);
			case 401: 
				return new UnauthorizedError(status, error, tag);
			case 403: 
				return new ForbiddenError(status, error, tag);
			case 404: 
				return new NotFoundError(status, error, tag);
			default: 
				return new RestifizerError(status, error, tag);
			}
		}
	}
}
