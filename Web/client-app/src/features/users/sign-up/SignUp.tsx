import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { usePostJson } from "@/api/usePostJson.ts";
import { Title } from "@/components/text/Title.tsx";
import { Button } from "@/components/ui/button.tsx";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormLabel,
  FormMessage,
} from "@/components/ui/form.tsx";
import { Input } from "@/components/ui/input.tsx";
import { useToast } from "@/components/ui/use-toast.ts";
import type { User } from "@/features/users/user.ts";

type CreateUserRequest = {
  name: string;
  email: string;
};

const signUpFormSchema = z.object({
  name: z.string().min(3).max(20),
  email: z.string().email(),
});

export const SignUp = () => {
  const { toast } = useToast();

  const createUser = usePostJson<CreateUserRequest, User>(
    "api/user/CreateUser",
  );

  const form = useForm<z.infer<typeof signUpFormSchema>>({
    resolver: zodResolver(signUpFormSchema),
    defaultValues: {
      name: "",
      email: "",
    },
  });

  const onSubmit = (formValues: z.infer<typeof signUpFormSchema>) => {
    const user = {
      name: formValues.name,
      email: formValues.email,
    } as CreateUserRequest;

    void createUser.makeRequest({
      requestBody: user,
      onSuccess: (createdUser) => {
        toast({
          title: "User created",
          description: `User ${createdUser.name} created successfully`,
        });
      },
      onFailure: (err) => {
        toast({
          title: "User creation failed",
          description: err,
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
