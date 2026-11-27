import { useState } from "react";

export default function ContactForm() {
  const [message, setMessage] = useState("");
  function submit(event) {
    event.preventDefault();
    setMessage("Your details are valid. This demonstration form does not send a message.");
  }
  return <section id="contact" aria-labelledby="contact-title">
    <p className="eyebrow">GET IN TOUCH</p><h2 id="contact-title">Contact</h2>
    <form onSubmit={submit}>
      <div className="field"><label htmlFor="contact-name">Name</label><input id="contact-name" name="name" autoComplete="name" minLength="2" required /></div>
      <div className="field"><label htmlFor="contact-email">Email</label><input id="contact-email" name="email" type="email" autoComplete="email" required /></div>
      <div className="field"><label htmlFor="contact-message">Message</label><textarea id="contact-message" name="message" minLength="10" required /></div>
      <p className="hint">This is a demonstration form. No message will be sent.</p>
      <button type="submit">Validate message</button>
      {message && <p role="status">{message}</p>}
    </form>
  </section>;
}
