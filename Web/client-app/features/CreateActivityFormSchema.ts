import { z } from "zod";

export const CreateActivityFormSchema = z.object({
  title: z.string().min(1, "Title is required").max(200, "Title is too long"),
  distanceInMeters: z.number().positive("Distance must be positive"),
  dateOfActivity: z
    .string()
    .min(1, "Date is required")
    .regex(/^\d{4}-\d{2}-\d{2}$/, "Date must be in YYYY-MM-DD format"),
});

export type CreateActivityFormValues = z.infer<typeof CreateActivityFormSchema>;
