import {
  FormControl,
  FormControlError,
  FormControlErrorIcon,
  FormControlErrorText,
  FormControlHelper,
  FormControlHelperText,
  FormControlLabel,
  FormControlLabelText,
} from "../ui/form-control";
import { Input, InputField } from "../ui/input";
import { FieldValues, Path, UseFormRegister } from "react-hook-form";

type TextInputProps<T extends FieldValues> = {
  label: Path<T>;
  register: UseFormRegister<T>;
};
export const TextField = <T extends FieldValues>({
  label,
  register,
}: TextInputProps<T>) => (
  <FormControl>
    <FormControlLabel>
      <FormControlLabelText className="text-typography-900">
        {label}
      </FormControlLabelText>
    </FormControlLabel>
    <Input className="my-1 bg-background-100 border border-primary-500 rounded" size="md">
      <InputField
        {...register(label)}
        className="text-typography-900 placeholder:text-typography-500"
      />
    </Input>
    <FormControlHelper>
      <FormControlHelperText />
    </FormControlHelper>
    <FormControlError>
      <FormControlErrorIcon />
      <FormControlErrorText />
    </FormControlError>
  </FormControl>
);
