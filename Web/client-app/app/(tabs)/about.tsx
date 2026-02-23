import { Text, View } from "react-native";
import { CreateActivityFormSchema } from "@/features/CreateActivityFormSchema";
import { useForm } from "react-hook-form";
import { TextField } from "@/components/form/TextField";

// ToDo isd - rename
export default function AboutScreen() {
  const createActivityForm = useForm<CreateActivityFormSchema>({
    defaultValues: {
      title: "",
      description: null,
      dateOfActivity: "",
    },
  });

  return (
    <View className="flex-1 bg-background-800 justify-center items-center">
      <Text className="text-typography-900 text-xl mb-4">Add activity</Text>
      <form>
        <TextField label="title" register={createActivityForm.register} />
      </form>
    </View>
  );
}
