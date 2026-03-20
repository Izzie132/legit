import { ActivityIndicator, Text, View } from "react-native";
import {
  CreateActivityFormSchema,
  CreateActivityFormValues,
} from "@/features/CreateActivityFormSchema";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation } from "@tanstack/react-query";
import { TextField } from "@/components/form/TextField";
import { DecimalInput } from "@/components/form/DecimalInput";
import { useApiClient } from "@/api/useApiClient";
import Button from "@/components/Button";

export default function ActivityScreen() {
  const apiClient = useApiClient();

  const { control, handleSubmit } = useForm<CreateActivityFormValues>({
    resolver: zodResolver(CreateActivityFormSchema),
    defaultValues: {
      title: "",
      name: "",
      distanceInMeters: 0,
      dateOfActivity: "",
    },
  });

  const mutation = useMutation({
    mutationFn: (data: CreateActivityFormValues) =>
      apiClient.createActivity({
        activity: {
          ...data,
          userId: "",
        },
      }),
  });

  const onSubmit = handleSubmit((data) => mutation.mutate(data));

  return (
    <View className="flex-1 bg-brand-background justify-center items-center">
      <Text className="text-white text-xl mb-4">Add Activity</Text>
      <View className="w-[320px] gap-3 items-center">
        <TextField label="Title" name="title" control={control} />
        <TextField label="Name" name="name" control={control} />
        <DecimalInput
          label="Distance (meters)"
          name="distanceInMeters"
          control={control}
          placeholder="e.g. 5000"
        />
        <TextField
          label="Date of Activity"
          name="dateOfActivity"
          control={control}
          placeholder="YYYY-MM-DD"
        />
        {mutation.isError && (
          <Text className="text-error-500 text-center">
            {mutation.error?.message ?? "Something went wrong"}
          </Text>
        )}
        {mutation.isSuccess && (
          <Text className="text-primary-500 text-center">
            Activity created successfully!
          </Text>
        )}
        {mutation.isPending ? (
          <ActivityIndicator />
        ) : (
          <Button label="Submit" theme="primary" onPress={onSubmit} />
        )}
      </View>
    </View>
  );
}
