# Making API Requests

This project is configured to make API requests using the [auto-generated API client](../Backend/API-Client-Generation.md)
from the backend, in conjunction with [TanStack Query](https://tanstack.com/query/latest) as an async state management library.

## API Client

The generated typescript API client contains the methods and types needed to make API requests. It can be initialised
by creating an instance of the client class with the API base URL.

Once this has been created, its methods can simply be called to make the corresponding API request. The result will either
successfully return the response, or it will throw an `ApiException`. In the case that the error was expected, the `result`
member of this error will contain the body of the non 200 response. If the error was not described in the Swagger documentation,
this exception will be thrown without a result, and with a generic message.

An example of how to use this API client is shown below. In reality, we will always be calling these methods from within
a `useQuery` or `useMutation` hook. To learn more about these, read the TanStack Query section of this document.

```typescript
const apiClient = new ApiClient(window.location.origin);

try {
  const users = await apiClient.getUsers();
  console.log(users.users.length);
} catch (e) {
  if (ApiException.isApiException(e)) {
    console.log(e.message);
    console.log(e.result); // only included for defined errors
  }
}
```

## TanStack Query

TanStack Query is a library that provides a set of hooks to manage async state in a React application. It is used in this
project to manage the state of API requests.

### useQuery

`useQuery` is a hook that is used to fetch data from an API. It takes a query key and a function that will be called to
fetch the data. The function should return a promise that resolves to the data that should be stored in the query cache.

```typescript
const getWeather = useQuery({
  queryKey: ["getWeather"],
  queryFn: ({ signal }) => apiClient.getWeather(signal),
});

const { data, isLoading, isError, error } = getWeather;
```

### useMutation

`useMutation` is a hook that is used to send data to an API. It takes a mutation key and a function that will be called to
send the data. The function takes in the data to send and should return a promise that resolves to the response from the API.

```typescript
const createUser = useMutation({
  mutationFn: (user: CreateUserRequest) => apiClient.createUser(user),
});

const { mutate, isLoading, isError, error } = createUser;

mutate(user, {
  onSuccess: (createdUser) => {
    toast({
      title: "User created",
      description: `User ${createdUser.name} created successfully`,
    });
  },
  onError: (err) => {
    toast({
      title: "User creation failed",
      description: parseApiException(err).userVisibleMessage,
      variant: "destructive",
    });
  },
});
```
