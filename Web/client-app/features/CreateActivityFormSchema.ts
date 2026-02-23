import { z } from "zod";
import zt from "zod-temporal";

const CreateActivityFormSchema = z.object({
  title: z.string(),
  description: z.string().nullable(),
  dateOfActivity: zt.plainDateTime(),
});

export type CreateActivityFormSchema = z.infer<typeof CreateActivityFormSchema>;
