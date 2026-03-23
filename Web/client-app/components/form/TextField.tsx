import { Control, Controller, FieldValues, Path } from "react-hook-form";
import { FormField, InputField } from "./FormField";
import type { KeyboardTypeOptions } from "react-native";

type TextFieldProps<T extends FieldValues> = {
  label: string;
  name: Path<T>;
  control: Control<T>;
  placeholder?: string;
  keyboardType?: KeyboardTypeOptions;
};

export const TextField = <T extends FieldValues>({
  label,
  name,
  control,
  placeholder,
  keyboardType,
}: TextFieldProps<T>) => (
  <Controller
    control={control}
    name={name}
    render={({ field: { onChange, onBlur, value }, fieldState: { error } }) => (
      <FormField label={label} error={error}>
        <InputField
          className="text-brand-background"
          value={String(value ?? "")}
          onChangeText={onChange}
          onBlur={onBlur}
          placeholder={placeholder}
          keyboardType={keyboardType}
          variant="outline"
        />
      </FormField>
    )}
  />
);
