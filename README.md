Restifizer SDK for Unity3D
==========

Restifizer - it's a way to significantly simplify creation of full-functional RESTful services, using MongoDB as a database.
And this is SDK that allows you to easily integrate this solution to your Unity3d games.
  
> Restifizer is available at https://github.com/vedi/restifizer

> For a quick start you can use our full functional seed-project based on MEAN.JS (http://meanjs.org/), it is available at https://github.com/vedi/restifizer-mean.js.

> The SDK is very young, and we continuously work to improve it. Any feedback is appreciated.

## Prerequisites

This SDK depends on UnityHTTP (https://github.com/andyburke/UnityHTTP). Please, integrate it to your project.

## Getting started

1. Add sources of SDK to your project.
1. Put `RestifizerManager` to your scene.
1. Set up `Base Url` you want to use in Inspector.
1. Optionally, set `Error Handler` draging there the script that implements `IErrorHandler` interface (see `Error Handling` for details).
1. Bind `RestifizerManager` to a script, where you want to use it (defining such property in the script, adn setting it in Inspector).

You're ready to make requests to Restifizer.

## Requests

RestifizerManager allows you to create `RestifizerRequest` with the following call:
 
```
RestifizerManager.ResourceAt(path)
```

For example,

```
RestifizerManager.ResourceAt("api/users")
```

`RestifizerRequest` has a set of self-explanatory methods which can be combined to a chain if calls. For example:

```
RestifizerManager.ResourceAt("api/users").SetFields("name").OrderBy("createdAt")
```

The example creates and configures the request to work with "api/users" path and set up `field` and `orderBy` params (see appropriate sections in Restifizer documentation https://github.com/vedi/restifizer#supported-params).

After you configured the request as a final touch you should specify http-method to call. The following methods are supported:
* GET,
* POST,
* PUT,
* PATCH,
* DELETE.

When you call GET you provide just `callback`, for other methods you additionaly provide `params` - 
Hashtable to be serialized to JSON and put to request body.

For example, getting list of the users:

```
RestifizerManager.ResourceAt("api/users").
        SetFields("name").
        OrderBy("createdAt").
        Get((response) => {
            // response.ResourceList - at this point you can use the result
        });
```

And another example, signing up an user:

```
        Hashtable parameters = new Hashtable();
        parameters.Add("username", "test");
        parameters.Add("password", "test");

        RestifizerManager.ResourceAt("api/users").Post(parameters, (response) => {
            // response.Resource - at this point you can use the result 
        });

```

## Accessing to exact resources

There is a lot of cases, where we need to specify ID of an affected resource in our request. 
In REST API such IDs are specified as an additional part of resource path. 
In Restifizer SDK in order to focus a request on an exact record you should call `One` method, passing ID there.
    
For example, we are changing an user's password:
```
        Hashtable parameters = new Hashtable();
        parameters.Add("password", "newPassword");

        RestifizerManager.ResourceAt("api/users").One(userId).Patch(parameters, (response) => {
            // response.Resource - at this point you can use the result 
        });
        
```

## Authentication

Restifizer SDK support 2 kinds of authentications: 
* Client Credentials (https://tools.ietf.org/html/rfc6749#section-1.3.4)
* Access Token (https://tools.ietf.org/html/rfc6749#section-1.4)
 
In order to use it in your request you need to configure `RestifizerManager` before.

### Client Credentials

Configuration:

```
RestifizerManager.ConfigClientAuth(CLIENT_ID, CLIENT_SECRET);
```

Using (add `WithClientAuth()` to the chain):

```
RestifizerManager.ResourceAt("api/users").
        WithClientAuth().
        SetFields("name").
        OrderBy("createdAt").
        Get((response) => {
            // response.ResourceList - at this point you can use the result
        });
```

### Access Token

Configuration:

```
RestifizerManager.ConfigBearerAuth(accessToken);
```

Using (add `WithBearerAuth()` to the chain):

```
        RestifizerManager.ResourceAt("api/users").
                WithBearerAuth().
                One(userId).
                Patch(parameters, (response) => {
                    // response.Resource - at this point you can use the result 
                });
```

## Response

Response from the server is wrapped with `RestifizerResponse` that provides you with parsed data. 
Depending on context of request it can be a resource or a list of resources. 
There are 2 appropriate properties for this: `response.Resource` - `Hashtable`, or `response.ResourceList` - `ArrayList` of `Hashtable`s.
    
## Error Handling

There are 2 approaches to handle error in the SDK: via EventHandler, and in callbacks of requests.

If you set `ErrorHandler` of `RestifizerManager` with instance of `IErrorHandler`, every error will pass through its method
`bool onRestifizerError(RestifizerError restifizerError)`. 
If you want to suppress further handling in `callback` of the request, you should return `false` in this method.
Otherwise, your callback will be called, with `Error` propery of response set to instance of `RestifizerError`.
  
For example,

```
    public bool onRestifizerError(RestifizerError restifizerError) {
        Debug.Log(restifizerError.Message);
        return true;    // do not stop propagating
    }

    public void ExitGame() {

        Hashtable parameters = new Hashtable();
        . . .
        RestifizerManager.ResourceAt("api/games/").One(gameId).WithBearerAuth().Patch(parameters, (response) => {
          if (response.HasError) {
            // handle response.Error here
          }
        });
    }
```


