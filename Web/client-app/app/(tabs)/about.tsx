import { Text, View, StyleSheet, TextInput } from "react-native";
import theme from "@/constants/theme";
import { CreateActivityFormSchema } from "@/features/CreateActivityFormSchema";
import { Form, useForm } from "react-hook-form";
import { TextField } from "@/components/form/TextInput";
import { register } from "node:module";

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
    <View style={styles.container}>
      <Text style={styles.text}>Add activity</Text>
      <form>
        <TextField label="title" register={createActivityForm.register} />
      </form>
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: theme.Colors.background,
    justifyContent: "center",
    alignItems: "center",
  },
  text: {
    color: theme.Colors.textPrimary,
  },
});
