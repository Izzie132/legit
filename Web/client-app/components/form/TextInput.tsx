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
export const TextInput = <T extends FieldValues>({
  label,
  register,
}: TextInputProps<T>) => (
  <FormControl>
    <FormControlLabel>
      <FormControlLabelText />
    </FormControlLabel>
    <Input className="my-1" size="md">
      <InputField {...register(label)} />
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
