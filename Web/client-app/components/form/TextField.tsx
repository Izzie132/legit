import {
  FormControl,
  FormControlError,
  FormControlErrorText,
  FormControlLabel,
  FormControlLabelText,
} from "../ui/form-control";
import { Input, InputField } from "../ui/input";
import { Control, Controller, FieldValues, Path } from "react-hook-form";
import type { KeyboardTypeOptions } from "react-native";

type TextInputProps<T extends FieldValues> = {
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
}: TextInputProps<T>) => (
  <Controller
    control={control}
    name={name}
    render={({ field: { onChange, onBlur, value }, fieldState: { error } }) => (
      <FormControl isInvalid={!!error} className="w-full">
        <FormControlLabel>
          <FormControlLabelText className="text-white">
            {label}
          </FormControlLabelText>
        </FormControlLabel>
        <Input variant="outline" size="md" className="my-1 border-outline-400">
          <InputField
            className="text-brand-background"
            value={String(value ?? "")}
            onChangeText={onChange}
            onBlur={onBlur}
            placeholder={placeholder}
            keyboardType={keyboardType}
            variant="outline"
          />
        </Input>
        {error && (
          <FormControlError>
            <FormControlErrorText>{error.message}</FormControlErrorText>
          </FormControlError>
        )}
      </FormControl>
    )}
  />
);
