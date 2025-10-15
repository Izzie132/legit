import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { z } from "zod";
import type { CreateUserRequest } from "@/api/ApiClient";
import { parseApiException } from "@/api/apiErrorReponse";
import { useApiClient } from "@/api/useApiClient";
import { Title } from "@/components/text/Title";
import { Button } from "@/components/ui/button";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import { useToast } from "@/components/ui/use-toast";

const signUpFormSchema = z.object({
  name: z.string().min(3).max(20),
  email: z.email(),
});

export const SignUp = () => {
  const { toast } = useToast();

  const apiClient = useApiClient();

  const createUser = useMutation({
    mutationFn: (user: CreateUserRequest) => apiClient.createUser(user),
  });

  const form = useForm({
    resolver: zodResolver(signUpFormSchema),
    defaultValues: {
      name: "",
      email: "",
    },
  });

  const onSubmit = (formValues: z.infer<typeof signUpFormSchema>) => {
    const user: CreateUserRequest = {
      name: formValues.name,
      email: formValues.email,
    };

    createUser.mutate(user, {
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
  };

  return (
    <>
      <Title>Sign Up</Title>
      <Form {...form}>
        <form onSubmit={form.handleSubmit(onSubmit)} className="flex flex-col">
          <FormField
            control={form.control}
            name="name"
            render={({ field }) => (
              <FormItem className="mt-6">
                <FormLabel>Name</FormLabel>
                <FormControl>
                  <Input {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <FormField
            control={form.control}
            name="email"
            render={({ field }) => (
              <FormItem className="mt-6">
                <FormLabel>Email</FormLabel>
                <FormControl>
                  <Input {...field} />
                </FormControl>
                <FormMessage />
              </FormItem>
            )}
          />
          <Button className="mt-8" type="submit">
            Submit
          </Button>
        </form>
      </Form>
    </>
  );
};
