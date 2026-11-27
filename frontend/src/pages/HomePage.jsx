import { Link } from "react-router";
import Weather from "../components/Weather.jsx";
import ContactForm from "../components/ContactForm.jsx";

export default function HomePage() {
  return <>
    <section className="hero">
      <p className="eyebrow">SOFTWARE ENGINEERING STUDENT</p>
      <h1>Hello, I am<br /><span>Brian Arjun.</span></h1>
      <p className="lead">I build web applications and learn by making them work, from the interface to the database.</p>
      <div className="actions"><Link className="button" to="/projects">Explore my projects →</Link><Link className="button secondary" to="/blogposts">Read my notes</Link></div>
    </section>
    <section className="intro-grid" aria-label="About this portfolio">
      <div><p className="eyebrow">ABOUT ME</p><h2>Learning through building.</h2></div>
      <div><p>I study HBO-ICT Software Engineering at The Hague University of Applied Sciences. This portfolio brings together my projects and development notes.</p><p>The application uses React for the interface and ASP.NET Core with SQL Server for the API and data.</p></div>
    </section>
    <div className="home-extras"><Weather /><ContactForm /></div>
  </>;
}
