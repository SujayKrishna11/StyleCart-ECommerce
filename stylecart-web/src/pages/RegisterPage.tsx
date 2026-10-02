import type { FormEvent } from "react";
import { useState } from "react";
import { register } from "../api/styleCartApi";
import type { LoginResponse } from "../types/models";

type RegisterPageProps = {
    onRegister: (response: LoginResponse) => void;
};

function RegisterPage({ onRegister }: RegisterPageProps) {
    const [error, setError] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);

    async function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();

        const formData = new FormData(event.currentTarget);

        const firstName = String(formData.get("firstName"));
        const lastName = String(formData.get("lastName"));
        const email = String(formData.get("email"));
        const password = String(formData.get("password"));
        const confirmPassword = String(formData.get("confirmPassword"));

        if (password !== confirmPassword) {
            setError("Password and Confirm Password must match.");
            return;
        }

        try {
            setError("");
            setIsSubmitting(true);

            const response = await register(firstName, lastName, email, password);
            onRegister(response);
        } catch (requestError) {
            if (requestError instanceof Error) {
                setError(requestError.message);
            } else {
                setError("Registration failed. Please try again.");
            }
        } finally {
            setIsSubmitting(false);
        }
    }

    return (
        <section className= "form-page" >
        <div className="form-card" >
            <p className="eyebrow" > New customer </p>
                < h1 > Create account </h1>
                    < p > Register to add products to your cart and place orders.</p>

                        < form onSubmit = { handleSubmit } >
                            <label>
                            First name
                                < input name = "firstName" required />
                                    </label>

                                    <label>
            Last name
        < input name = "lastName" required />
            </label>

            <label>
    Email
        < input name = "email" type = "email" required />
            </label>

            <label>
    Password
        < input
    name = "password"
    type = "password"
    minLength = { 6}
    required
        />
        </label>

        <label>
            Confirm password
        < input
    name = "confirmPassword"
    type = "password"
    minLength = { 6}
    required
        />
        </label>

    { error && <p className="error-message" > { error } </p> }

    <button type="submit" disabled = { isSubmitting } >
    { isSubmitting? "Creating account...": "Create account" }
        </button>
        </form>
        </div>
        </section>
  );
}

export default RegisterPage;